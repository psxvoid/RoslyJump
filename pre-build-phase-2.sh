
activeBranch=$ACTIVEBRANCH

isMarketplaceRelease=${ISMARKETPLACERELEASE,,}
isReleaseCandidate=${ISRELEASECANDIDATE,,}

latestMajor=$LATESTMAJOR
latestMinor=$LATESTMINOR
latestPatch=$LATESTPATCH
revisionCounter=$REVISIONCOUNTER

if [[ -z $activeBranch ]]; then
    >&2 echo 'ENV Error: The ACTIVEBRANCH environment variable is not set.'
    exit 1
fi

if [[ -z $isMarketplaceRelease ]] || ! [[ $isMarketplaceRelease =~ (true|false) ]]; then
    >&2 echo 'ENV Error: The ISMARKETPLACERELEASE environment variable is not set.'
    exit 1
fi

if [[ -z $isReleaseCandidate ]] || ! [[ $isReleaseCandidate =~ (true|false) ]]; then
    >&2 echo 'ENV Error: The ISRELEASECANDIDATE environment variable is not set.'
    exit 1
fi

if [[ -z $latestMajor ]] || ! [[ $latestMajor =~ ^[0-9]+$ ]]; then
    >&2 echo 'ENV Error: The LATESTMAJOR environment variable is not set or is not a number.'
    exit 1
fi

if [[ -z $latestMinor ]] || ! [[ $latestMinor =~ ^[0-9]+$ ]]; then
    >&2 echo 'ENV Error: The LATESTMINOR environment variable is not set or is not a number.'
    exit 1
fi

if [[ -z $latestPatch ]] || ! [[ $latestPatch =~ ^[0-9]+$ ]]; then
    >&2 echo 'ENV Error: The LATESTPATCH environment variable is not set or is not a number.'
    exit 1
fi

if [[ -z $revisionCounter ]] || ! [[ $revisionCounter =~ ^[0-9]+$ ]]; then
    >&2 echo 'ENV Error: The REVISIONCOUNTER environment variable is not set or is not a number.'
    exit 1
fi

releaseVersion="$latestMajor.$latestMinor.$latestPatch.$revisionCounter"

# update vsixmanifest version
if [[ $activeBranch == "main" && $isMarketplaceRelease = true ]]; then
    releaseVersion="$latestMajor.$latestMinor.$latestPatch"
fi

sed -r -i.bak "s/(Identity..*Version=\")([[:digit:]]*\.[[:digit:]]*\.[[:digit:]]*)(\")/\1$releaseVersion\3/g" ./RoslyJump/source.extension.vsixmanifest

# verify the version number is correctly updated in the vsixmanifest
grep -Eq "Identity.*Version=\"$releaseVersion\"" ./RoslyJump/source.extension.vsixmanifest \
    && success=true || success=false

if [[ $success = false ]]; then
    >&2 echo 'Pre-build Error: Unable to update VSIX version number.'
    exit 1
fi

echo "Release Version:      $releaseVersion"
echo "Is Marketplace-Ready: $isMarketplaceRelease"
echo "Is Release Candidate: $isReleaseCandidate"

echo "##vso[task.setvariable variable=releaseVersion;isOutput=true]$releaseVersion"