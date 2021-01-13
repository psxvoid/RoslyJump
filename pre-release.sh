varsJson=$(cat build-results.json)

isMarketplaceRelease=$(echo $varsJson | grep -Po "isMarketplaceRelease:\s+(.*?)[,}]" | grep -Po "(false|true)")
isReleaseCandidate=$(echo $varsJson | grep -Po "isReleaseCandidate:\s+(.*?)[,}]" | grep -Po "(false|true)")
releaseVersion=$(echo $varsJson | grep -Po "releaseVersion:\s+(.*?)[,}]" | grep -Po "(\d+\.\d+.\d+(\.\d+)?)")


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