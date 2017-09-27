GRANT - GRafische ANwendungen auf Taktilen Displays
=========
GRANT ist ein Framework um die Inklusion von Blinden und Sehbehinderten zu verbessern. 
Mithilfe des Frameworks sollen es für einen Assistenten möglich sein eine Desktop-Anwendung mit grafischer Benutzeroberfläche für ein taktiles Ausgabegerät (Braille Display) aufzubereiten. Die Benutzung dieses Software Development Kit (SDK) soll sehr einfach sein und keine Programmierkenntnisse vom Assistenten erfordern.

# Installation
Um das Framwork zu nutzen werden noch die folgenden Komponenten benötigt:
* [BrailleIO-Framework](https://github.com/TUD-INF-IAI-MCI/BrailleIO)
* [MVBD](http://download.metec-ag.de/MVBD/)
* [Screenreader NVDA](https://www.nvaccess.org/download/) (optional)


Damit GRANT ausgeführt werden kann, muss das BrailleIO-Projekt in Visual Studio verlinkt werden. Verlinkungen müssen dabei zu den folgenden Projekten gesetzt werden:
- BrailleIO
- BrailleIO_Interfaces
- BrailleIO_ShowOff
- BrailleRenderer
- GestureRecognition

Damit auch das BrailleDis (spezielles Braille Display) über BrailleIO angesprochen werden kann, müssen die Projekte HBBrailleDisV2 und BrailleIOBraillDisAdapter auch hinzugefügt werden. Diese sind nicht im offiziellen git der TU Dresden enthalten! Anschließend müssen die folgenden Verlinkungen in Visual Studio bei BrailleIOBraillDisAdapter gesetzt werden.
- BrailleIO
- BrailleIO_Interfaces
- HBBrailleDisV2

Alternativ kann eine beliebiges Braille Display auch mithilfe des Treibers für MVBD (BrailleIOBraillDisAdapterMVBD -- im git enthalten) angesprochen werden.