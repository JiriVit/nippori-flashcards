# nippori-flashcards
Flashcard application for exercising foreign languages.

**Platform:** .NET 4.6.1

**Programming language:** C# with Windows Presentation Foundation

**IDE:** Visual Studio 2017 Community Edition

## Summary

This is a flashcards program, supposed to help people with exercising foreign languages. You prepare Excel file with vocabulary and the program will show it to you and ask for translation. There are lots of such programs in the Internet, I tried a few of them and they didn't satisfy me and I am not willing to try more of them, because I am a software engineer, so why not practice C# and make my own stuff.

The application is named Nippori, because I like travelling to Japan, especially Tokyo, and [Nippori](https://en.wikipedia.org/wiki/Nippori_Station) is one of my favorite Tokyo train stations. It is not my top favorite, that is [Minami-Senju](https://en.wikipedia.org/wiki/Minami-Senju_Station), because it is close to hotel [Juyoh](http://www.juyoh.co.jp/) where I usually stay, but I already used the name Minami-Senju for my another project, so I had to use my second favorite station for this flashcards app. I like the name, because it sounds cool Japan and I like the station, because it is close to [Yanaka](https://en.wikipedia.org/wiki/Yanaka_Cemetery) area and it is also terminal of [Nippori-Toneri Liner](https://en.wikipedia.org/wiki/Nippori-Toneri_Liner), an automated guideway transit system, which is fancy name for a train without driver. I like those Japanese hi-tech transportation systems, that's why I created this app, so I can learn Japanese and next time in Japan speak to the locals and understand local writings like a boss. You should go to Japan and visit Tokyo, I strongly recommend it.

Please note there are following limitations in this project:
* There are now two projects in the solution: "Nippori" and "NipporiWtf".
* Project "Nippori" is done in Windows Forms (which means that it is obsolete crap and won't be updated) and it is completely in Czech language (texts in application + comments in code), because it was originally a private project, so I had no reason to keep it in English.
* Project "NipporiWtf" is done in WPF (which means that it will be cutting-edge, if it is ever finished) and it has GUI in English, but code is mostly commented in Czech, and also not very well developed. My intention is to rework the original WF app in WPF, but I am lazy and spend too much time playing videogames on my 5 consoles and almost no time coding. I started working on this remaster only because I registered to JLPT N5 and I needed a real flashcards app (without text entry, only showing things) and I refused to add this feature to the WF crap, so I started the WPF rework, and I also use this coding as procrastination, when I don't want to learn for JLPT.

Aside from removing those limitations, I have following items on my TODO list:
* Choose format of input data (vocabulary). Now it is done in Excel, which is cool for editing, why do own GUI when Microsoft and his open-source plagiators already did the job, but Excel has its disadvantages, it takes long to load to the app and it is proprietary  binary format, I would like to share my vocabularies here as well, but I don't want to put binary files here, because that's heresy. But if  I choose a text format like XML or CSV, I lose the comfort of Excel, eg. CSVs cannot have fancy formatting. We'll see about this.
* Add some metrics. I would like to see how much of the vocabulary I already practised, the app knows it, but doesn't show it, so there should be some charts or tables to show the progress.
