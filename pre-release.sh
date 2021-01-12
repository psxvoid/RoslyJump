if [[ -z $isMarketplaceRelease || "$isMarketplaceRelease" = false ]] ; then
    >&2 echo 'Pre-Release Error: The build is not marked as marketplace-ready.'
    exit 1
fi