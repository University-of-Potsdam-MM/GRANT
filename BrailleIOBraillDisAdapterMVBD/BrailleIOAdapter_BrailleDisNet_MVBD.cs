using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using BrailleIO.Interface;
using BrailleIO;




namespace BrailleIOBraillDisAdapter
{
    public class BrailleIOAdapter_BrailleDisNet_MVBD : AbstractBrailleIOAdapterBase, IDisposable
    {
        protected IPEndPoint        _ep;
        protected TcpClient         _tcpClient;


        public BrailleIOAdapter_BrailleDisNet_MVBD(IBrailleIOAdapterManager manager) : base(manager)
        {
            _ep         = new IPEndPoint(IPAddress.Loopback, 2017);

            ThreadPool.QueueUserWorkItem( new WaitCallback( Thread_Callback ) );
            while (this.Device == null) { }//warten bis DeviceInfos abgerufen wurden

        }

        ~BrailleIOAdapter_BrailleDisNet_MVBD()
        {
            Dispose();
        }

        public void Dispose()
        {
            if ( ( _tcpClient != null ) && ( _tcpClient.Connected == true ) )
            {
                _tcpClient.Close();
            }
        }


        public override bool Connect()
        {
            return base.Connect();
        }

        public override bool Disconnect()
        {
            Dispose();

            return base.Disconnect();
        }

        public override void Synchronize(bool[,] matrix)
        {
            // Nicht aufrufen!
            //base.Synchronize(matrix);

            if ( LockPins == false )
            {
                SendPins(matrix);
            }
        }

        /// <summary>Working Thread</summary>
        protected void Thread_Callback(object o)
        {
            while(true)
            {

                try
                {
                    if ( _tcpClient == null )
                    {
                        _tcpClient  = new TcpClient();
                        _tcpClient.ReceiveTimeout = 500;
                        _tcpClient.SendTimeout    = 500;
                    }


                    if ( _tcpClient.Connected == false )
                    {
                        TryConnect();

                        if ( _tcpClient.Connected == true )
                        {
                            SendGetDeviceInfo();
                        }

                    }




                    if ( _tcpClient.Connected == true )
                    {

                        if ( _tcpClient.Available > 3)
                        {
                            NetworkStream ns = _tcpClient.GetStream();
                            int cmd  = ns.ReadByte();
                            int lenL = ns.ReadByte();
                            int lenH = ns.ReadByte();
                            int len  = lenH << 8 | lenL;

                            byte[] ba = new byte[len];
                            ns.Read(ba,0,len);


                            if (cmd == 20)
                            {
                                int workingposition = ba[2];
                                if ( (workingposition == 0) || (workingposition == 2) )
                                {
                                    // TODO
                                    //_graphicDisplay.PinCountX = ba[0];
                                    //_graphicDisplay.PinCountY = ba[1];
                                }
                                else
                                {
                                    // TODO
                                    //_graphicDisplay.PinCountX = ba[1];
                                    //_graphicDisplay.PinCountY = ba[0];
                                }
                                this.Device = new BrailleIODevice(ba[0], ba[1], "MVBD_" + ba[4], true, true, 30); //TODO: Ausrichtung beachten; ab Name einträge dynamisch bestimmen
                                //_formMain.Draw();

                                Debug.Print ("--> DeviceInfo {0}x{1}", ba[0], ba[1]);
                            }

                            else if (cmd == 22)
                            {
                                int key = ba[0];
                                System.Collections.Specialized.OrderedDictionary raw = new System.Collections.Specialized.OrderedDictionary();

                                if      ( ( key == 224 ) || ( key == 226 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.ZoomInDown,  ref raw);    // Zoom+
                                else if ( ( key == 225 ) || ( key == 227 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.ZoomOutDown, ref raw);    // Zoom-
                                else if ( ( key == 208 ) || ( key == 216 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.UpDown,      ref raw);    // Up
                                else if ( ( key == 209 ) || ( key == 217 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.LeftDown,    ref raw);    // Left
                                else if ( ( key == 210 ) || ( key == 218 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.DownDown,    ref raw);    // Down
                                else if ( ( key == 211 ) || ( key == 219 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.RightDown,   ref raw);    // Right
                                else if ( ( key == 212 ) || ( key == 220 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.EnterDown,   ref raw);    // Enter

                                else Debug.Print ("--> KeyDown {0}", ba[0]);
                            }

                            else if (cmd == 23)
                            {

                                int key = ba[0];
                                System.Collections.Specialized.OrderedDictionary raw = new System.Collections.Specialized.OrderedDictionary();

                                if      ( ( key == 224 ) || ( key == 226 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.ZoomInUp,  ref raw);    // Zoom+
                                else if ( ( key == 225 ) || ( key == 227 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.ZoomOutUp, ref raw);    // Zoom-
                                else if ( ( key == 208 ) || ( key == 216 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.UpUp,      ref raw);    // Up
                                else if ( ( key == 209 ) || ( key == 217 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.LeftUp,    ref raw);    // Left
                                else if ( ( key == 210 ) || ( key == 218 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.DownUp,    ref raw);    // Down
                                else if ( ( key == 211 ) || ( key == 219 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.RightUp,   ref raw);    // Right
                                else if ( ( key == 212 ) || ( key == 220 ) )    fireKeyStateChanged(BrailleIO_DeviceButtonStates.EnterUp,   ref raw);    // Enter
                                else Debug.Print ("--> KeyUp {0}", ba[0]);


                                //else if ( ( key == 208 ) || ( key == 216 ) )    _formMain.CommandUp         ();      // Up
                                //else if ( ( key == 209 ) || ( key == 217 ) )    _formMain.CommandLeft       ();      // Left
                                //else if ( ( key == 210 ) || ( key == 218 ) )    _formMain.CommandDown       ();      // Down
                                //else if ( ( key == 211 ) || ( key == 219 ) )    _formMain.CommandRight      ();      // Right
                                //else if ( ( key == 212 ) || ( key == 220 ) )    _formMain.CommandEnter      ();      // Enter




                                //else if ( key == 228 )                          _formMain.CommandMoveUp1    (null);  // 1x MoveUp
                                //else if ( key == 229 )                          _formMain.CommandMoveLeft1  (null);  // 1x MoveLeft
                                //else if ( key == 230 )                          _formMain.CommandMoveDown1  (null);  // 1x MoveDown
                                //else if ( key == 231 )                          _formMain.CommandMoveRight1 (null);  // 1x MoveRight

                                //else if ( key == 232 )                          _formMain.CommandMoveUp2    (null);  // 1x MoveUp
                                //else if ( key == 233 )                          _formMain.CommandMoveLeft2  (null);  // 1x MoveLeft
                                //else if ( key == 234 )                          _formMain.CommandMoveDown2  (null);  // 1x MoveDown
                                //else if ( key == 235 )                          _formMain.CommandMoveRight2 (null);  // 1x MoveRight



                                //else if ( key == 237 ) _formMain.CommandMove0Zoom1 (null);  // Home
                                //else if ( key == 236 ) _formMain.CommandEsc        ();      // ESC

                                //else
                                //{
                                //   Debug.Print ("--> KeyUp TODO {0}", ba[0]);
                                //}

                            }



                            else
                            {
                                Debug.Print ("--> Unknown Cmd={0} Len={1}", cmd, len);
                            }



                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }

                }
                catch(Exception ex)
                {
                    Debug.Print(ex.Message);
                }


            }

        }

        private void TryConnect ()
        {
            try
            {
                _tcpClient.Connect( _ep );
            }

            catch(SocketException ex)
            {
                Debug.Print(ex.Message);
            }

            catch(Exception ex)
            {
                _tcpClient.Close();                 // altes Socket schließen
                _tcpClient = null;

                _tcpClient = new TcpClient( _ep );  // close bringt hier nix
                Debug.Print(ex.Message);
            }
        }




        public void SendGetDeviceInfo()
        {
            if ( ( _tcpClient == null ) || ( _tcpClient.Connected == false ) )   return; // -->

            byte[] ba = new byte[3];
            ba[0] = 20;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print ("<-- SendGetDeviceInfo");
            Send(ba);
        }

        /// <summary>Send Pins X/Y gedreht!</summary>
        /// <param name="pins"></param>
        public void SendPins(bool[,] pins)
        {
            if ( ( _tcpClient == null ) || ( _tcpClient.Connected == false ) )    return; // -->

            //Rectangle rect = new Rectangle(0,0, 120, 60);
            Rectangle rect = new Rectangle(0, 0, this.Device.DeviceSizeX, this.Device.DeviceSizeY);
            byte[] ba = new byte[ 7 + (rect.Width * rect.Height / 8) + 1 ]; // 7 + 104 * 60 / 8 + 1 = 788 Bytes

            int lenData = ba.Length-3;        // Gesamtlänge - 3 Header
            ba[0] = 21;                       // Command 21
            ba[1] = (byte)( lenData >> 0 );   // Length low byte   = 16
            ba[2] = (byte)( lenData >> 8 );   // Length high Byte  = 3 * 256

            ba[3] = (byte)rect.Left;
            ba[4] = (byte)rect.Top;
            ba[5] = (byte)rect.Right;
            ba[6] = (byte)rect.Bottom;

            byte[] v2 = new byte[] {1,2,4,8,16,32,64,128};

            int cnt = (7 << 3); // Start bei Index 7

            for(int y = rect.Top; y < rect.Bottom && y < pins.GetLength(0); y++) // 0...59
            { 
                for (int x = rect.Left; x < rect.Right && x < (pins.Length /pins.GetLength(0)); x++)    // 0...104
                {
                    if ( pins[y,x] == true )
                    {
                        int i = (cnt >> 3);     // 4 4 4 4 4 4 4 4  5 5 5 5 5 5 5 5  6 6 6 6 6 6 6 6
                        int b = (cnt & 7);      // 0 1 2 3 4 5 6 7  0 1 2 3 4 5 6 7  0 1 2 3 4 5 6 7

                        ba[i] |= v2[b];
                    }

                    cnt++;
                }
            }

            //Debug.Print ("<-- SendPins Len={0} Rect={1} Thread={2}", ba.Length, rect, Thread.CurrentThread.ManagedThreadId);
            Send(ba);
        }




        public void Send (byte[] ba)
        {
            try
            {
                NetworkStream ns = _tcpClient.GetStream();
                ns.Write( ba, 0, ba.Length );
                ns.Flush();
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
            }
        
        }























    }
}
