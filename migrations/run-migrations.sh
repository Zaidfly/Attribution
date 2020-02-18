#! /bin/bash

set -eo pipefail

if [ -z "${CON_STR}" ]; then
	echo "Enviroment CON_STR does not exist"
	
	exit 1
fi

if [ -z "${M_TAG}" ]; then
	echo "Enviroment M_TAG does not exist"
	
	exit 1
fi

if [[ ! "${M_TAG}" =~ ^(Pre|Post)$ ]]; then
  echo "M_TAG must be Pre or Post value"
  
  exit 1
fi

echo "Tag: ${M_TAG}"
echo "Running migrations..."

dotnet --info

dotnet Migrations.Attribution.dll migrate -c "$CON_STR" --singleTransaction --tags $M_TAG

echo "Done"
