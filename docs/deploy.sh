#!/usr/bin/env sh

set -e

rm -rf docs/.vuepress/dist
npm install
npm run docs:build

cd docs/.vuepress/dist

git init
git add --all
git commit -m 'deploy'
git push -f https://github.com/runceel/ReactiveProperty.git main:gh-pages
