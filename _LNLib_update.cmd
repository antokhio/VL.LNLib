@echo off
echo Updating LNLib submodule...

git submodule update --init --recursive
cd lnlib
git pull origin main
cd ..

echo LNLib update complete.
pause