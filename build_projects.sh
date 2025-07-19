#!/bin/bash

# This script builds all .NET projects in the src directory and its subdirectories.
# Ensure the script is run from the root directory of the repository
if [ ! -d "./src" ]; then
    echo "The 'src' directory does not exist. Please run this script from the root of the repository."
    exit 1
fi
touch
listedProjects=$(find ./src -name "*.csproj")
if [ -z "$listedProjects" ]; then
    echo "No projects found."
    exit 0
fi

for project in $listedProjects
do
    echo "Found project [$project]"
    echo
    echo "Building project [$project]"
    echo

    dotnet build "$project" --configuration Release
    if [ $? -ne 0 ]; then
        echo "Build failed for project: [$project]"
        echo "Please check the project for errors."
        echo "Exiting script."
        exit 1
    fi

    VERSION_FILE_PATH="$(dirname "$project")"
    PROJECT_VERSION=$(nbgv get-version --project "$VERSION_FILE_PATH")

    dotnet pack "$project" --configuration Release --output ./artifacts
    if [ $? -ne 0 ]; then
        echo "Pack failed for project: [$project]\n"
        echo "Please check the project for errors."
        echo "Exiting script."
        exit 1
    fi

    echo "Pack completed for project: [$project]"
    echo
    echo "Version: $PROJECT_VERSION"
    echo "Artifacts are located in ./artifacts"
    echo "Build and pack completed for project: [$project]"
    echo 
    echo
done

echo "All projects have been built and packed successfully."
echo "Artifacts are located in ./artifacts"