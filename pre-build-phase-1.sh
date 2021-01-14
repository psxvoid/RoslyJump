if [[ -z $ISPULLREQUEST ]]; then
    >&2 echo 'ENV Error: The environment variable ISPULLREQUEST is not set.'
    exit 1
fi

isPullRequest="${ISPULLREQUEST,,}"

if ! [[ $isPullRequest =~ (true|false) ]]; then
    >&2 echo 'ENV Error: The environment variable ISPULLREQUEST is not a boolean.'
    exit 1
fi

activeBranch=$(git rev-parse --abbrev-ref HEAD)

releaseGlob="[[:digit:]]*.[[:digit:]]*.[[:digit:]]*"
releaseGrepRegex="^[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+$"
releaseCandidateGrepRegex="^[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+-rc[[:digit:]]\+$"

# get the latest tag on the current branch
latestReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

# get the tag before the latest on the current branch
previousReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 2 | tail -n 1)

# get the latest release-candidate tag on the current branch
latestReleaseCandidateTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseCandidateGrepRegex | head -n 1)

# get the candidate tag before the latest on the current branch
previousReleaseCandidateTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseCandidateGrepRegex | head -n 2 | tail -n 1)

# get the latest tag on the main branch
# latestReleaseMainTag=$(git tag --list $releaseGlob --merged main --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

releaseTagOnLatestCommit=$(git tag --contains HEAD --list $releaseGlob | grep $releaseGrepRegex | head -n 1)
releaseCandidateTagOnLatestCommit=$(git tag --contains HEAD --list $releaseGlob | grep $releaseCandidateGrepRegex | head -n 1)

echo "Latest Release:        ${latestReleaseTag:-empty}"
echo "Previous Release:      ${previousReleaseTag:-empty}"
echo "Latest RC:             ${latestReleaseCandidateTag:-empty}"
echo "Previous RC:           ${previousReleaseCandidateTag:-empty}"
echo "Latest Commit Release: ${releaseTagOnLatestCommit:-empty}"
echo "Latest Commit RC:      ${releaseCandidateTagOnLatestCommit:-empty}"

if [[ ! -z $releaseTagOnLatestCommit && ! -z $releaseCandidateTagOnLatestCommit ]]; then
    >&2 echo "Tag Error: The build can be marked as release or release-candidate (not both)."
    exit 1
fi

if [[ $activeBranch != "main" && ! -z $releaseTagOnLatestCommit ]]; then
    >&2 echo "Tag Error: Marking a build as realease is only allowed on the main branch."
    exit 1
fi

if [[ $activeBranch != "main" && ! -z $releaseCandidateTagOnLatestCommit ]]; then
    >&2 echo "Tag Error: Marking a build as realease-candidate is only allowed on the main branch."
    exit 1
fi

isMarketplaceRelease=false
isReleaseCandidate=false

if [[ $activeBranch = "main" ]]; then
    if [[ ! -z $releaseTagOnLatestCommit && $isPullRequest = false ]]; then
        isMarketplaceRelease=true
    elif [[ ! -z $releaseCandidateTagOnLatestCommit && $isPullRequest = false ]]; then
        isReleaseCandidate=true
    fi
fi

currentMajor=`echo $latestReleaseTag | cut -d. -f1`
currentMinor=`echo $latestReleaseTag | cut -d. -f2`
currentPatch=`echo $latestReleaseTag | cut -d. -f3`

previousMajor=`echo $previousReleaseTag | cut -d. -f1`
previousMinor=`echo $previousReleaseTag | cut -d. -f2`
previousPatch=`echo $previousReleaseTag | cut -d. -f3`

latestReleaseCandidateTag="${latestReleaseCandidateTag//-rc/.}"
currentRcMajor=`echo $latestReleaseCandidateTag | cut -d. -f1`
currentRcMinor=`echo $latestReleaseCandidateTag | cut -d. -f2`
currentRcPatch=`echo $latestReleaseCandidateTag | cut -d. -f3`
currentRc=`echo $latestReleaseCandidateTag | cut -d. -f4`

previousReleaseCandidateTag="${previousReleaseCandidateTag//-rc/.}"
previousRcMajor=`echo $previousReleaseCandidateTag | cut -d. -f1`
previousRcMinor=`echo $previousReleaseCandidateTag | cut -d. -f2`
previousRcPatch=`echo $previousReleaseCandidateTag | cut -d. -f3`
previousRc=`echo $previousReleaseCandidateTag | cut -d. -f4`

isRcGreater=false
isRcEqual=false

if (( currentMajor <= currentRcMajor )); then
    isRcGreater=true
else
    if (( currentMinor <= currentRcMinor )); then
        isRcGreater=true
    else
        if (( currentPatch < currentRcPatch )); then
            isRcGreater=true
        elif (( currentPatch == currentRcPatch )); then
            isRcEqual=true
        fi
    fi
fi

isPreviousRcGreater=false

if (( previousMajor <= currentRcMajor )); then
    isPreviousRcGreater=true
else
    if (( previousMinor <= currentRcMinor )); then
        isPreviousRcGreater=true
    else
        if (( previousPatch < currentRcPatch )); then
            isPreviousRcGreater=true
        fi
    fi
fi

if [[ $isRcGreater = true ]]; then
    currentMajor=$currentRcMajor
    currentMinor=$currentRcMinor
    currentPatch=$currentRcPatch
fi

if [[ $isPreviousRcGreater = true ]]; then
    previousMajor=$previousRcMajor
    previousMinor=$previousRcMinor
    previousPatch=$previousRcPatch
fi

if (( currentMajor == previousMajor )); then
    if (( currentMinor == previousMinor )); then
        if (( currentPatch < previousPatch )); then
            >&2 echo "Versioning Error: Patch version is less than the previous one."
            exit 1
        elif [[ $currentPatch = $previousPatch ]]; then
            if (( currentRc < previousRc )); then
                >&2 echo "Versioning Error: Release candidate version is less than the previous one."
                exit 1
            elif (( currentRc != previousRc && currentRc != previousRc + 1 )); then
                >&2 echo "Versioning Error: Release candidate version is increased but not incremented."
                exit 1
            fi
        elif (( currentPatch != previousPatch + 1 )); then
            >&2 echo "Versioning Error: Patch version is increased but not incremented."
            exit 1
        fi
    else
        if (( currentMinor <= previousMinor )); then
            >&2 echo "Versioning Error: Minor version is less than the previous one."
            exit 1
        elif (( currentMinor != previousMinor + 1 )); then
            >&2 echo "Versioning Error: Minor version is increased but not incremented."
            exit 1
        fi
    fi
else
    if (( currentMajor < previouseMajor )); then
        >&2 echo "Versioning Error: Major version is increased but not incremented."
        exit 1;
    elif (( currentMajor != previousMajor + 1 )); then
        >&2 echo "Versioning Error: Major version is increased but not incremented."
        exit 1
    fi
fi

echo "##vso[task.setvariable variable=activeBranch;isOutput=true]$activeBranch"

echo "##vso[task.setvariable variable=isMarketplaceRelease;isOutput=true]$isMarketplaceRelease"
echo "##vso[task.setvariable variable=isReleaseCandidate;isOutput=true]$isReleaseCandidate"

echo "##vso[task.setvariable variable=latestMajor;isOutput=true]$currentMajor"
echo "##vso[task.setvariable variable=latestMinor;isOutput=true]$currentMinor"
echo "##vso[task.setvariable variable=latestPatch;isOutput=true]$currentPatch"

if [[ $isMarketplaceRelease = true ]]; then
    echo "##vso[build.addbuildtag]marketplace-release"
fi

if [[ $isReleaseCandidate = true ]]; then
    echo "##vso[build.addbuildtag]release-candidate"
fi
