releaseVersionRegex="^\d+\.\d+\.d+$"

preReleaseVersionRegex="^\d+\.\d+\.d+-rc\d+$"

activeBranch=$(git rev-parse --abbrev-ref HEAD)

releaseGlob="[[:digit:]]*.[[:digit:]]*.[[:digit:]]*"
releaseGrepRegex="^[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+$"

# get the latest tag on the current branch
latestReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

# get the tag before the latest on the current branch
previousReleaseTag=$(git tag --list $releaseGlob --sort=-creatordate | grep $releaseGrepRegex | head -n 2 | tail -n 1)

# get the latest tag on the main branch
latestReleaseMainTag=$(git tag --list $releaseGlob --merged main --sort=-creatordate | grep $releaseGrepRegex | head -n 1)

releaseTagOnLatestCommit=$(git tag --contains HEAD --list $releaseGlob | grep $releaseGrepRegex | head -n 1)

echo $latestReleaseTag
echo $previousReleaseTag
echo $latestReleaseMainTag
# echo $releaseTagOnLatestCommit

currentMajor=`echo $latestReleaseTag | cut -d. -f1`
currentMinor=`echo $latestReleaseTag | cut -d. -f2`
currentPatch=`echo $latestReleaseTag | cut -d. -f3`

previousMajor=`echo $previousReleaseTag | cut -d. -f1`
previousMinor=`echo $previousReleaseTag | cut -d. -f2`
previousPatch=`echo $previousReleaseTag | cut -d. -f3`

if (( currentMajor == previousMajor )); then
    if (( currentMinor == previousMinor )); then
        if (( currentPatch < previousPatch )); then
            >&2 echo "Versioning Error: Patch version is less than the previous one."
            exit 1
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

if [[ "$activeBranch" == "main" || true ]]; then
    echo main
    if [[ "$releaseTagOnLatestCommit" == "$latestReleaseMainTag" ]]; then
        # update vsixmanifest version
        sed -r -i.bak "s/(Identity..*Version=\")([[:digit:]]*\.[[:digit:]]*\.[[:digit:]]*)(\")/\1$currentMajor.$currentMinor.$currentPatch\3/g" ./RoslyJump/source.extension.vsixmanifest
    else
        # the "revision" environment variable should be set here
        revVal="${revision:-default_value}"
        
        if [[ -z revVal ]]; then
            >&2 echo "ENV Error: The revision counter environment variable is not set."
            exit 1
        fi

        if ! [[ $revVal =~ ^[0-9]+$ ]] ; then
            >&2 echo "ENV Error: The revision counter environment variable should be an integer number."
            exit 1
        fi

        sed -r -i.bak "s/(Identity..*Version=\")([[:digit:]]*\.[[:digit:]]*\.[[:digit:]]*)(\")/\1$currentMajor.$currentMinor.$currentPatch.$revVal\3/g" ./RoslyJump/source.extension.vsixmanifest
    fi
else
    # Do Something here
    echo not main
fi