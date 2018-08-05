# faq

## how to set browser engine settings

since the program use embeded firefox as browser engine 

you can set the firefox preference

the preference can find when you input `about:config` in firefox

then you can check the specific meaning by reference [official document](http://kb.mozillazine.org/Category:Preferences)

## how to reduce the size

currently, it has intergrate linux and windows firefox binary folder in this project, which loacte at src/WegouSharp/Resource/firefox_***
by default, the config file (wegousharpsettings.json) have the value `"UseEmbededBrowser":true` ,
youc can remove thoset file in resource/firefox_*** if you want to reduce the size . but make sure you have also to change to setting as `"UseEmbededBrowser":false` and have install firefox to your system(add to system path , so that yo can start firefox anywhere)
