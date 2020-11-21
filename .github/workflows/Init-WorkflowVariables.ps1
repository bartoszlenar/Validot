$commitShortSha = $env:GITHUB_SHA.Substring(0, 7)

if ($env:GITHUB_EVENT_NAME.Equals("release")) {

    $tag = $env:GITHUB_REF.Substring("refs/tags/".Length).TrimStart('v');

    if ($tag -match "^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")
    {
        $version = $tag
    }
    else
    {
        Write-Error "Tag contains invalid semver: $tag" -ErrorAction Stop
    }
}
else {
    $version = $commitShortSha
}

"VALIDOT_VERSION=$version" | Out-File -FilePath $env:GITHUB_PATH -Encoding utf8 -Append
"VALIDOT_COMMIT=$commitShortSha" | Out-File -FilePath $env:GITHUB_PATH -Encoding utf8 -Append
"VALIDOT_CI=true" | Out-File -FilePath $env:GITHUB_PATH -Encoding utf8 -Append