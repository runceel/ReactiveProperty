Remove-Item .\docs\.vuepress\dist
npm run docs:build

Set-Location docs/.vuepress/dist

git init
git add --all
git commit -m 'deploy'
git push -f https://github.com/runceel/ReactiveProperty.git master:gh-pages
