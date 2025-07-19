#!/bin/bash

# Defaults
CONFIGURATION="Release"
VERBOSITY="minimal"
PACKAGES_OUTPUT="./artifacts"

# Parse command-line arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -pc|--pack-output) PACKAGES_OUTPUT="$2"; shift ;;
        *) echo "Unknown parameter: $1" && exit 1 ;;
    esac
    shift
done

echo "Using configuration: $CONFIGURATION"
echo
echo "Using verbosity: $VERBOSITY"
echo
echo

# This script builds all .NET projects in the src directory and its subdirectories.
# Ensure the script is run from the root directory of the repository
if [ ! -d "./src" ]; then
    echo "The 'src' directory does not exist. Please run this script from the root of the repository."
    exit 1
fi

listedProjects=$(find ./src -name "*.csproj")
if [ -z "$listedProjects" ]; then
    echo "No projects found."
    exit 0
fi

for project in $listedProjects
do
    echo "Found project $project"
    echo
    echo "Building project $project"
    echo

    dotnet build "$project" --configuration "$CONFIGURATION" --verbosity "$VERBOSITY"
    if [ $? -ne 0 ]; then
        echo "Build failed for project $project"
        echo "Please check the project for errors."
        echo "Exiting script."
        exit 1
    fi

    VERSION_FILE_PATH="$(dirname "$project")"
    PROJECT_VERSION=$(nbgv get-version -p "$VERSION_FILE_PATH" --variable Version)
    echo "Version File Path: $VERSION_FILE_PATH"
    echo "Project Version: $PROJECT_VERSION"

    dotnet pack "$project" --configuration "$CONFIGURATION" --output "$PACKAGES_OUTPUT" /p:Version="$PROJECT_VERSION" --verbosity "$VERBOSITY"
    if [ $? -ne 0 ]; then
        echo "Pack failed for project $project"
        echo "Please check the project for errors."
        echo "Exiting script."
        exit 1
    fi

    echo "Pack completed for project $project"
    echo
    echo "$project $PROJECT_VERSION has been packed in $PACKAGES_OUTPUT"
    echo "Build and pack completed for project $project"
    echo 
    echo
done

echo "All projects have been built and packed successfully."
echo "Packages artifacts are located in $PACKAGES_OUTPUT"