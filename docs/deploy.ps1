Remove-Item .\docs\.vuepress\dist
npm install
npm run docs:build

Set-Location docs/.vuepress/dist

git init
git add --all
git commit -m 'deploy'
git push -f https://github.com/runceel/ReactiveProperty.git main:gh-pages
