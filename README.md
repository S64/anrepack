# Anrepack

A wrapper tool of [Apktool](http://ibotpeaches.github.io/Apktool/) for automating apk re-packaging operation, for Windows, macOS.

## Installation

You can get the latest builds at [Releases](https://github.com/S64/anrepack/releases).  
If you don't installed [.NET Core Runtime](https://dotnet.microsoft.com/download), Please use `*-selfcontained.zip` version.

## Usages

```sh
./anrepack --help
# Usage: anrepack [options] [command]
# 
# Options:
#   -?|-h|--help             Show help information
# 
# Commands:
#   generate-debug-keystore  Generate "debug.keystore" to default location.
#   install-android          Download Android SDK to default location.
#   install-apktool          Download Apktool to Anrepack's temporary location.
#   repack                   Execute repackage operation.
#   version                  Show version.
# 
# Run 'anrepack [command] --help' for more information about a command.
```

```sh
ls ../mywork
# operation.py	original.apk

./anrepack repack --apk ../mywork/original.apk --script ../mywork/operation.py --output ../mywork/repacked.apk --my-script-arg=World
# ...
# Decode apk using apktool...
# ...
# Decoded.
# Run script...
# Hello World!
# Done.
# Re-Build apk using apktool...
# ...
# Built.
# Signing...
# ...
# Signed.
# Completed!

ls ../mywork
# operation.py	original.apk	repacked.apk
```

<details>
<summary>cat ../mywork/operation.py</summary>

```python
-*- coding: utf-8 -*-

import argparse

parser = argparse.ArgumentParser()
parser.add_argument('--my-script-arg', dest='myScriptArg', required=True)

def processDecodedApk(anrepackVersion, decodedPath):
    args = parser.parse_args()
    print('Hello %s!' % args.myScriptArg)
```

</details>
