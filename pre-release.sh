prodPath="$SYSTEM_DEFAULTWORKINGDIRECTORY/vsix/prod/build-results.json"
rcPath="$SYSTEM_DEFAULTWORKINGDIRECTORY/vsix/rc/build-results.json"
ciPath="$SYSTEM_DEFAULTWORKINGDIRECTORY/vsix/ci/build-results.json"

targetStage="Unknown"
matches=0

if [[ -f "$ciPath" ]]; then
    targetStage="Development"
    matches=$((matches+1))
fi

if [[ -f "$rcPath" ]]; then
    targetStage="Release Candidate"
    matches=$((matches+1))
fi

if [[ -f "$prodPath" ]]; then
    targetStage="Production"
    matches=$((matches+1))
fi

echo "Target stage: $targetStage"
echo "Stages count: $matches"

if (( $matches > 1 )); then
    >&2 echo "Pre-Release Error: Only a single-stage artifacts are supported."
    exit 1
fi

if [[ $targetStage != "Production" ]]; then
    >&2 echo "Pre-Release Error: The production artifact is missing. Actual: $targetStage."
    exit 1
fi

varsJson=$(cat $prodPath)

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