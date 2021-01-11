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
echo $releaseTagOnLatestCommit


if [[ "$activeBranch" == "main" ]]; then
    echo main
    if [[ "$releaseTagOnLatestCommit" == "$latestReleaseMainTag" ]]; then
        # Do something here
        echo true
    else
        # Do Something here
        echo false
    fi
else
    # Do Something here
    echo not main
fi