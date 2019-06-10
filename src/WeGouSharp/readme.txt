!!!! Important Note for NuGet User !!!!
!!!! Nuget用户重要说明 !!!!

Since dotnetcore/dotnet cli(for nuget) don't support distributing conent and tools anymore
Actually, you still can do that, but they file is just a link from cache. And there's many potential issues.

You may need to add Resource file manually.

Please download resource from GitHub and put it to published folder or add to your project and publish it 

Resource mentioned here include some configurations and dependences like driver , etc


由于dotnetcore/dotnet cli用于nuget的接口现在不支持发布静态资源文件和工具集了（其实也能发布，只是拿到的是一链接文件，又有一堆坑，已经不建议这么使用）

因此你需要手动添加可能需要的资源文件

资源文件可从GitHub仓库下载, 请把下载的资源文件放在发布目录下，
或者添加到你的项目，然后作为发布文件输出

这里所说的资源包括一些配置文件，依赖项如gecko驱动之类的东西。


