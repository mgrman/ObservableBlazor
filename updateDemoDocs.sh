dotnet publish src/ObservableBlazor.Example/ObservableBlazor.Example.csproj -c Release -o docs
cp -rl docs/wwwroot docs
rm -r docs/wwwroot
rm -r docs/web.config
sed -i -e 's/<base href=\"\/\" \/>/<base href=\"\/ObservableBlazor\/\" \/>/g' docs/index.html