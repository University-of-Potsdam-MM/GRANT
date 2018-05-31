using System;


namespace TactileWeb
{

    /// <summary>A Filter two values, Active and Anchor booleans</summary>
    public class Filter
    {
        protected bool _active;
        protected bool _anchor;
        protected int  _v1;
        protected int  _v2;



        /// <summary>Creates a new Filter</summary>
        public Filter(bool active, bool anchor, int v1, int v2)
        {
            _active  = active;
            _anchor  = anchor;
            _v1      = v1;
            _v2      = v2;
        }

        /// <summary>Information of the object like "80-120"</summary>
        public override string ToString()
        {
            return String.Format("{0}-{1}",  _v1, _v2);
        }


        /// <summary>Use the filter</summary>
        public bool Active { get { return _active; }    set{ _active = value; } }

        /// <summary>If true the other value will be changed too</summary>
        public bool Anchor { get { return _anchor; }    set{ _anchor = value; } }

        /// <summary>Value 1 (10)</summary>
        public int V1
        {
            get
            {
                return _v1;
            }
            set
            {
                int v1 = _v1;
                int v2 = _v2;

                if ( _anchor == true )
                {
                    int change = (value - v1);

                    v1 += change;
                    v2 += change;
                }

                else
                {
                    v1 = value;
                }

                SetValues( v1, v2);
            }

        }



        /// <summary>Value 2 (20)</summary>
        public int V2
        {
            get
            {
                return _v2;
            }
            set
            {
                int v1 = _v1;
                int v2 = _v2;

                if ( _anchor == true )
                {
                    int change = (value - v2);

                    v1 += change;
                    v2 += change;
                }

                else
                {
                    v2 = value;
                }

                SetValues( v1, v2);

            }

        }




        /// <summary>Check and set the values without error</summary>
        protected bool SetValues(int v1, int v2)
        {
            int minimum = 0;
            int maximum = 255;

            if (v1 > v2     )   return false;   // --> v1 greater than v2

            if (v1 < minimum)   return false;   // --> v1 to small
            if (v2 < minimum)   return false;   // --> v2 to small

            if (v1 > maximum)   return false;   // --> v1 to big
            if (v2 > maximum)   return false;   // --> v2 to big


            _v1 = v1;
            _v2 = v2;
            return true;
        }


    }
}