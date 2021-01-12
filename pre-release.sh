# to lowercase
isMarketplaceRelease="${ISMARKETPLACERELEASE,,}"

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