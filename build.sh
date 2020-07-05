#! /bin/sh
set -e

RED='\e[0;31m'
GREEN='\e[0;32m'
NC='\e[m'

version=$1

[[ -z ${version} ]] && echo -e "${RED}Error: You must provide a version${NC}" && exit 1
re='^[0-9]+([.][0-9]+)*$'
[[ ! ${version} =~ ${re} ]] && echo -e "${RED}Error: '${version}' is not a valid version string${NC}" && exit 1

dotnet test
dotnet tool update --global Amazon.Lambda.Tools
dotnet lambda package --project-location ./src/ApiGatewayCustomAuthorizer --msbuild-parameters -p:Version=${version}
mkdir -p artifacts
cp ./src/ApiGatewayCustomAuthorizer/bin/Release/netcoreapp3.1/ApiGatewayCustomAuthorizer.zip ./artifacts/custom-authorizer-${version}.zip

echo -e "${GREEN}Successfully built and packaged version ${version}${NC}"
