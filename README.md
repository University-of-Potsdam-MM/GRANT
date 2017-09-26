GRANT - GRafische ANwendungen auf Taktilen Displays
=========
GRANT ist ein Framework um die Inklusion von Blinden und Sehbehinderten zu verbessern. 
Mithilfe des Frameworks sollen es f�r einen Assistenten m�glich sein eine Desktop-Anwendung mit grafischer Benutzeroberfl�che f�r ein taktiles Ausgabeger�t (Braille Display) aufzubereiten. Die Benutzung dieses Software Development Kit (SDK) soll sehr einfach sein und keine Programmierkenntnisse vom Assistenten erfordern.

# Installation
Um das Framwork zu nutzen werden noch die folgenden Komponenten ben�tigt:
* [BrailleIO-Framework](https://github.com/TUD-INF-IAI-MCI/BrailleIO)
* [MVBD](http://download.metec-ag.de/MVBD/)
* [Screenreader NVDA](https://www.nvaccess.org/download/) (optional)


Damit GRANT ausgef�hrt werden kann, muss das BrailleIO-Projekt in Visual Studio verlinkt werden. Verlinkungen m�ssen dabei zu den folgenden Projekten gesetzt werden:
- BrailleIO
- BrailleIO_Interfaces
- BrailleIO_ShowOff
- BrailleRenderer
- GestureRecognition

Damit auch das BrailleDis (spezielles Braille Display) �ber BrailleIO angesprochen werden kann, m�ssen die Projekte HBBrailleDisV2 und BrailleIOBraillDisAdapter auch hinzugef�gt werden. Diese sind nicht im offiziellen git der TU Dresden enthalten! Anschlie�end m�ssen die folgenden Verlinkungen in Visual Studio bei BrailleIOBraillDisAdapter gesetzt werden.
- BrailleIO
- BrailleIO_Interfaces
- HBBrailleDisV2

Alternativ kann eine beliebiges Braille Display auch mithilfe des Treibers f�r MVBD (BrailleIOBraillDisAdapterMVBD -- im git enthalten) angesprochen werden.