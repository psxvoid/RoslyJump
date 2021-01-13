varsJson=$(cat $SYSTEM_DEFAULTWORKINGDIRECTORY/RoslyJump.Vsix/RoslyJump.Vsix/build-results.json)

isMarketplaceRelease=$(echo $varsJson | grep -Eo "isMarketplaceRelease:[[:space:]]+(.*)[,}]" | grep -Eo "(false|true)" | head -n 1)
isReleaseCandidate=$(echo $varsJson | grep -Eo "isReleaseCandidate:[[:space:]]+(.*)[,}]" | grep -Eo "(false|true)" | head -n 1)
releaseVersion=$(echo $varsJson | grep -Eo "releaseVersion:[[:space:]]+(.*)[,}]" | grep -Eo "([[:digit:]]+\.[[:digit:]]+.[[:digit:]]+(\.[[:digit:]]+)?)" | head -n 1)


echo "Release Version:      $releaseVersion"
echo "Is Marketplace-Ready: $isMarketplaceRelease"
echo "Is Release Candidate: $isReleaseCandidate"

if [[ -z $isMarketplaceRelease ]]; then
    >&2 echo 'ENV Error: The environment variable ISMARKETPLACERELEASE is not set.'
    exit 1
fi

if ! [[ $isMarketplaceRelease =~ (true|false) ]]; then
    >&2 echo 'ENV Error: The environment variable ISMARKETPLACERELEASE should be a boolean.'
    exit 1
fi

if [[ $isMarketplaceRelease = false ]]; then
    >&2 echo 'Pre-Release Error: The build is not marked as marketplace-ready.'
    exit 1
fi