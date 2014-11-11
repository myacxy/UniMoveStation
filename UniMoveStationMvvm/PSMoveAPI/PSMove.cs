/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.9
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace io.thp.psmove
{

    using System;
    using System.Runtime.InteropServices;
    using UniMove;

    public class PSMove : IDisposable
    {
        protected HandleRef move;
        protected bool swigCMemOwn;

        public PSMove()
        {

        }

        public PSMove(int id) : this(pinvoke.psmove_connect_by_id(id), true)
        {

        }

        internal PSMove(IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            move = new HandleRef(this, cPtr);
        }

        internal static HandleRef getCPtr(PSMove obj)
        {
            return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.move;
        }

        ~PSMove()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                if (move.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        pinvoke.delete_PSMove(move);
                    }
                    move = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
            }
        }

        public PSMoveConnectionType ConnectionType
        {
            get
            {
                return pinvoke.PSMove_connection_type_get(move);
            }
        }

        public int ax
        {
            get
            {
                return pinvoke.PSMove_ax_get(move);
            }
        }

        public int ay
        {
            get
            {
                return pinvoke.PSMove_ay_get(move);
            }
        }

        public int az
        {
            get
            {
                return pinvoke.PSMove_az_get(move);
            }
        }

        public int gx
        {
            get
            {
                return pinvoke.PSMove_gx_get(move);
            }
        }

        public int gy
        {
            get
            {
                int ret = pinvoke.PSMove_gy_get(move);
                return ret;
            }
        }

        public int gz
        {
            get
            {
                return pinvoke.PSMove_gz_get(move);
            }
        }

        public int mx
        {
            get
            {
                return pinvoke.PSMove_mx_get(move);
            }
        }

        public int my
        {
            get
            {
                return pinvoke.PSMove_my_get(move);
            }
        }

        public int mz
        {
            get
            {
                return pinvoke.PSMove_mz_get(move);
            }
        }

        public void get_accelerometer(out int x, out int y, out int z)
        {
            pinvoke.psmove_get_accelerometer(move, out x, out y, out z);
        }

        public void get_accelerometer_frame(PSMoveFrame frame, out float arg1, out float arg2, out float arg3)
        {
            pinvoke.PSMove_get_accelerometer_frame(move, (int)frame, out arg1, out arg2, out arg3);
        }

        public void get_gyroscope(out int x, out int y, out int z)
        {
            pinvoke.psmove_get_gyroscope(move, out x, out y, out z);
        }
        public void get_gyroscope_frame(PSMoveFrame frame, out float arg1, out float arg2, out float arg3)
        {
            pinvoke.PSMove_get_gyroscope_frame(move, (int) frame, out arg1, out arg2, out arg3);
        }

        public void get_magnetometer(out int x, out int y, out int z)
        {
            pinvoke.psmove_get_magnetometer(move, out x, out y, out z);
        }

        public void get_magnetometer_vector(out float x, out float y, out float z)
        {
            pinvoke.PSMove_get_magnetometer_vector(move, out x, out y, out z);
        }

        public void enable_orientation(PSMoveBool enabled)
        {
            pinvoke.PSMove_enable_orientation(move, enabled);
        }

        public PSMoveBool has_orientation()
        {
            return pinvoke.PSMove_has_orientation(move);
        }

        public PSMoveBool has_calibration()
        {
            return pinvoke.PSMove_has_calibration(move);
        }

        public void get_orientation(out float w, out float x, out float y, out float z)
        {
            pinvoke.PSMove_get_orientation(move, out w, out x, out y, out z);
        }

        public void reset_orientation()
        {
            pinvoke.PSMove_reset_orientation(move);
        }

        public void set_leds(int r, int g, int b)
        {
            pinvoke.PSMove_set_leds(move, r, g, b);
        }

        public void set_rumble(int rumble)
        {
            pinvoke.PSMove_set_rumble(move, rumble);
        }

        public int update_leds()
        {
            return pinvoke.PSMove_update_leds(move);
        }

        public void set_rate_limiting(int enabled)
        {
            pinvoke.PSMove_set_rate_limiting(move, enabled);
        }

        public int pair()
        {
            return pinvoke.PSMove_pair(move);
        }

        public int pair_custom(string btaddr)
        {
            return pinvoke.PSMove_pair_custom(move, btaddr);
        }

        public string get_serial()
        {
            return pinvoke.PSMove_get_serial(move);
        }

        public int is_remote()
        {
            return pinvoke.PSMove_is_remote(move);
        }

        public int poll()
        {
            return pinvoke.PSMove_poll(move);
        }

        public uint get_buttons()
        {
            return pinvoke.PSMove_get_buttons(move);
        }

        public void get_button_events(out uint arg0, out uint arg1)
        {
            pinvoke.PSMove_get_button_events(move, out arg0, out arg1);
        }

        public PSMoveBatteryLevel get_battery()
        {
            return pinvoke.PSMove_get_battery(move);
        }

        public int get_temperature()
        {
            return pinvoke.PSMove_get_temperature(move);
        }

        public float get_temperature_in_celsius()
        {
            return pinvoke.PSMove_get_temperature_in_celsius(move);
        }

        public int get_trigger()
        {
            return pinvoke.PSMove_get_trigger(move);
        }

        public static int count_connected()
        {
            return pinvoke.count_connected();
        }

        public void connect()
        {
            swigCMemOwn = true;
            move = new HandleRef(this, pinvoke.new_PSMove__SWIG_0());
        }

        public void connect_by_id(int id)
        {
            swigCMemOwn = true;
            move = new HandleRef(this, pinvoke.new_PSMove__SWIG_1(id));
        }

        public void disconnect()
        {
            pinvoke.psmove_disconnect(move);
        }

        public void reinit()
        {
            pinvoke.psmove_reinit();
        }
    } //PSMove
} //namespace
