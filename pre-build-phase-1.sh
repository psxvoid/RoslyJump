if [[ -z $ISPULLREQUEST ]]; then
    >&2 echo 'ENV Error: The environment variable ISPULLREQUEST is not set.'
    exit 1
fi

isPullRequest="${ISPULLREQUEST,,}"

if ! [[ $isPullRequest =~ (true|false) ]]; then
    >&2 echo 'ENV Error: The environment variable ISPULLREQUEST is not a boolean.'
    exit 1
fi

if [[ -z $BUILD_SOURCEBRANCH ]]; then
    >&2 echo 'ENV Error: The environment variable BUILD_SOURCEBRANCH is not set.'
    exit 1
fi

# refs/heads/master
# refs/pull/1/merge
# refs/tags/your-tag-name
activeBranch=$BUILD_SOURCEBRANCH

releaseGlob="[[:digit:]]*.[[:digit:]]*.[[:digit:]]*"
releaseGrepRegex="^[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+\(.[[:digit:]]\+\)\?$"
releaseCandidateGrepRegex="^[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+\(.[[:digit:]]\+\)\?-rc[[:digit:]]\+$"

# get the latest tag on the current branch
latestReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

# get the tag before the latest on the current branch
previousReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 2 | tail -n 1)

# get the latest release-candidate tag on the current branch
latestReleaseCandidateTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseCandidateGrepRegex | head -n 1)

# get the candidate tag before the latest on the current branch
previousReleaseCandidateTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseCandidateGrepRegex | head -n 2 | tail -n 1)

# get the latest release tag on the main branch
latestReleaseMainTag=$(git tag --list $releaseGlob --merged main --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

# get the latest release-candidate tag on the main branch
latestReleaseCandidateMainTag=$(git tag --list $releaseGlob --merged main --sort=-creatordate | grep $releaseCandidateGrepRegex | head -n 1)

releaseTagOnLatestCommit=$(git tag --contains HEAD --list $releaseGlob | grep $releaseGrepRegex | head -n 1)
releaseCandidateTagOnLatestCommit=$(git tag --contains HEAD --list $releaseGlob | grep $releaseCandidateGrepRegex | head -n 1)

echo "Latest Release:        ${latestReleaseTag:-empty}"
echo "Previous Release:      ${previousReleaseTag:-empty}"
echo "Latest RC:             ${latestReleaseCandidateTag:-empty}"
echo "Previous RC:           ${previousReleaseCandidateTag:-empty}"
echo "Latest Main Release:   ${latestReleaseMainTag:-empty}"
echo "Latest Main RC:        ${latestReleaseCandidateMainTag:-empty}"
echo "Latest Commit Release: ${releaseTagOnLatestCommit:-empty}"
echo "Latest Commit RC:      ${releaseCandidateTagOnLatestCommit:-empty}"
echo "Active Branch:         $activeBranch"

if [[ ! -z $releaseTagOnLatestCommit && ! -z $releaseCandidateTagOnLatestCommit ]]; then
    >&2 echo "Tag Error: The build can be marked as release or release-candidate (not both)."
    exit 1
fi

isMarketplaceRelease=false
isReleaseCandidate=false

if [[ ! -z $releaseTagOnLatestCommit || ! -z $releaseCandidateTagOnLatestCommit ]]; then
    if [[ $activeBranch = "refs/heads/main" || $activeBranch = "refs/tags/$releaseTagOnLatestCommit" || $activeBranch = "refs/tags/$releaseCandidateTagOnLatestCommit" ]]; then
        if [[ ! -z $releaseTagOnLatestCommit && $releaseTagOnLatestCommit = $latestReleaseMainTag && $isPullRequest = false ]]; then
            # this is release-tag branch, e.g. tags/1.0.0 (azure pipelines tag trigger)
            # or main branch with the latest release-tag (azure pipelines manual trigger)
            isMarketplaceRelease=true
        elif [[ ! -z $releaseTagOnLatestCommit && ! $releaseTagOnLatestCommit = $latestReleaseMainTag ]]; then
            >&2 echo "Tag Error: Marking a build as realease is only allowed on the main branch (a tag branch but version is not equal to main)."
            exit 1
        fi

        if [[ ! -z $releaseCandidateTagOnLatestCommit && $releaseCandidateTagOnLatestCommit = $latestReleaseCandidateMainTag && $isPullRequest = false ]]; then
            isReleaseCandidate=true
        elif [[ ! -z $releaseCandidateTagOnLatestCommit && ! $releaseCandidateTagOnLatestCommit = $latestReleaseCandidateMainTag ]]; then
            >&2 echo "Tag Error: Marking a build as realease-candidate is only allowed on the main branch (a tag branch but version is not equal to main)."
            exit 1
        fi
    else
        if [[ ! -z $releaseTagOnLatestCommit ]]; then
            >&2 echo "Tag Error: Marking a build as realease is only allowed on the main branch (not main or tag branch)."
            exit 1
        fi

        if [[ ! -z $releaseCandidateTagOnLatestCommit ]]; then
            >&2 echo "Tag Error: Marking a build as realease-candidate is only allowed on the main branch (not main or tag branch)."
            exit 1
        fi
    fi
fi

echo "Is Marketplace-Ready: $isMarketplaceRelease"
echo "Is Release Candidate: $isReleaseCandidate"

currentMajor=`echo $latestReleaseTag | cut -d. -f1`
currentMinor=`echo $latestReleaseTag | cut -d. -f2`
currentPatch=`echo $latestReleaseTag | cut -d. -f3`
currentBuild=`echo $latestReleaseTag | cut -d. -f4`

if [[ -z $currentBuild ]]; then
    currentBuild=0
fi

previousMajor=`echo $previousReleaseTag | cut -d. -f1`
previousMinor=`echo $previousReleaseTag | cut -d. -f2`
previousPatch=`echo $previousReleaseTag | cut -d. -f3`
previousBuild=`echo $previousReleaseTag | cut -d. -f4`

if [[ -z $previousBuild ]]; then
    previousBuild=0
fi

echo "Current Major: $currentMajor"
echo "Current Minor: $currentMinor"
echo "Current Patch: $currentPatch"
echo "Current Build: $currentBuild"

echo "Previous Major: $previousMajor"
echo "Previous Minor: $previousMinor"
echo "Previous Patch: $previousPatch"
echo "Previous Build: $previousBuild"

latestReleaseCandidateTag="${latestReleaseCandidateTag//-rc/.}"
currentRcMajor=`echo $latestReleaseCandidateTag | cut -d. -f1`
currentRcMinor=`echo $latestReleaseCandidateTag | cut -d. -f2`
currentRcPatch=`echo $latestReleaseCandidateTag | cut -d. -f3`
currentRcBuild=`echo $latestReleaseCandidateTag | cut -d. -f4`
currentRc=`echo $latestReleaseCandidateTag | cut -d. -f5`

if [[ -z $currentRc ]]; then
    currentRc=currentRcBuild
    currentRcBuild=0
fi

previousReleaseCandidateTag="${previousReleaseCandidateTag//-rc/.}"
previousRcMajor=`echo $previousReleaseCandidateTag | cut -d. -f1`
previousRcMinor=`echo $previousReleaseCandidateTag | cut -d. -f2`
previousRcPatch=`echo $previousReleaseCandidateTag | cut -d. -f3`
previousRcBuild=`echo $previousReleaseCandidateTag | cut -d. -f4`
previousRc=`echo $previousReleaseCandidateTag | cut -d. -f5`

if [[ -z $previousRc ]]; then
    previousRc=previousRcBuild
    previousRcBuild=0
fi

echo "Current RC Major: $currentMajor"
echo "Current RC Minor: $currentMinor"
echo "Current RC Patch: $currentPatch"
echo "Current RC Build: $currentBuild"

echo "Previous RC Major: $previousMajor"
echo "Previous RC Minor: $previousMinor"
echo "Previous RC Patch: $previousPatch"
echo "Previous RC Build: $previousBuild"

isRcGreater=false
isRcEqual=false

if (( currentMajor <= currentRcMajor )); then
    isRcGreater=true
else
    if (( currentMinor <= currentRcMinor )); then
        isRcGreater=true
    else
        if (( currentPatch <= currentRcPatch )); then
            isRcGreater=true
        else
            if (( currentBuild < currentRcBuild )); then
                isRcGreater=true
            elif (( currentBuild == currentRcBuild )); then
                isRcEqual=true
            fi
        fi
    fi
fi

echo "Is RC Greater: $isRcGreater"
echo "Is RC Equal  : $isRcEqual"

isPreviousRcGreater=false

if (( previousMajor <= currentRcMajor )); then
    isPreviousRcGreater=true
else
    if (( previousMinor <= currentRcMinor )); then
        isPreviousRcGreater=true
    else
        if (( previousPatch <= currentRcPatch )); then
            isPreviousRcGreater=true
        else
          if (( previousBuild < currentRcBuild )); then
              isPreviousRcGreater=true
          fi
        fi
    fi
fi

echo "Is Previous RC Greater: $isPreviousRcGreater"

if [[ $isRcGreater = true ]]; then
    currentMajor=$currentRcMajor
    currentMinor=$currentRcMinor
    currentPatch=$currentRcPatch
    currentBuild=$currentRcBuild
fi

if [[ $isPreviousRcGreater = true ]]; then
    previousMajor=$previousRcMajor
    previousMinor=$previousRcMinor
    previousPatch=$previousRcPatch
    previousBuild=$previousRcBuild
fi

echo "Current Version : $currentMajor.$currentMinor.$currentPatch.$currentBuild"
echo "Previous Version: $previousMajor.$previousMinor.$previousPatch.$previousBuild"

if (( currentMajor == previousMajor )); then
    if (( currentMinor == previousMinor )); then
        if (( currentPatch == previousPatch )); then
            if (( currentBuild < previousBuild )); then
                >&2 echo "Versioning Error: Build version is less than the previous one."
                exit 1
            elif [[ $currentBuild = $previousBuild ]]; then
                if (( currentRc < previousRc )); then
                    >&2 echo "Versioning Error: Release candidate version is less than the previous one."
                    exit 1
                elif (( currentRc != previousRc && currentRc != previousRc + 1 )); then
                    >&2 echo "Versioning Error: Release candidate version is increased but not incremented."
                    exit 1
                fi
            elif (( currentBuild != previousBuild + 1 )); then
                >&2 echo "Versioning Error: Build version is increased but not incremented."
                exit 1
            fi
        else
            if (( currentPatch <= previousPatch )); then
                >&2 echo "Versioning Error: Patch version is less than the previous one."
                exit 1
            elif (( currentPatch != previousPatch + 1 )); then
                >&2 echo "Versioning Error: Patch version is increased but not incremented."
                exit 1
            fi
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
echo "##vso[task.setvariable variable=latestBuild;isOutput=true]$currentBuild"

if [[ $isMarketplaceRelease = true ]]; then
    echo "##vso[build.addbuildtag]marketplace-release"
fi

if [[ $isReleaseCandidate = true ]]; then
    echo "##vso[build.addbuildtag]release-candidate"
fi

if [[ $isReleaseCandidate = false && $isMarketplaceRelease = false ]]; then
    echo "##vso[build.addbuildtag]continious-integration"
fi
