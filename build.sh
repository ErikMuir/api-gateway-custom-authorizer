#! /bin/sh
set -e

version=$1

[[ -z ${version} ]] && echo "Error: You must provide a version" && exit 1
re='^[0-9]+([.][0-9]+)*$'
[[ ! ${version} =~ ${re} ]] && echo "Error: '${version}' is not a valid version string" && exit 1

dotnet test
dotnet tool update --global Amazon.Lambda.Tools
dotnet lambda package --project-location ./src/ApiGatewayCustomAuthorizer --msbuild-parameters -p:Version=${version}
mkdir -p artifacts
cp ./src/ApiGatewayCustomAuthorizer/bin/Release/netcoreapp3.1/ApiGatewayCustomAuthorizer.zip ./artifacts/custom-authorizer-${version}.zip

echo "Successfully built and packaged version ${version}"
