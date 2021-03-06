using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;


// ********************************************************
// Database connector and Insert scripting custom generated
// for the FT8OFF "Contest" by KD2FMW
// *********************************************************

namespace wsjt_message.Listener.Utils
{
    public class DataBase
    {
        static string ConnectionString = DBconnection();
        private static MySqlConnection conn;
        private static MySqlCommand cmd;

        public static string ConnectionString1 { get => ConnectionString; set => ConnectionString = value; }

        private static string DBconnection() //MySQL connection String
        {
            string username = "ft8off";
            string password = "T1p2Tip";
            string server = "localhost";
            string database = "ft8off";
            string myConnector = "user=" + username + ";password=" + password + ";server=" + server + ";port=3306;database=" + database;
            return myConnector;
        }
        public static void DBOpen()
        {
            conn = new MySqlConnection(ConnectionString);
            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void DBClose()
        {
            conn.Close();
        }
        public static void DBquery(string Query)
        {
            cmd = new MySqlCommand(Query, conn);
        }
        public static void ADIFinsert(Dictionary<string, string> adifout) // Need to seprate the array to field/value pairs
        {
            string call = "";
            string gridsquare = "";
            string mode = "";
            string rst_sent = "";
            string rst_rcvd = "";
            string qso_date = "";
            string time_on = "";
            string qso_date_off = "";
            string time_off = "";
            string band = "";
            string freq = "";
            string station_callsign = "";
            string my_gridsquare = "";
            string tx_pwr = "";
            int is_qrp = 0;
            string comment = "";
            string name = "";
            string operator_call = ""; // the ADIF field name operator is a reserved word
            string propmode = "";
            //string mode = "";
            //string mode = "";
            //string mode = "";
            //string mode = "";




            //STRING    call
            if (adifout.ContainsKey("call"))
            {
                call = adifout["call"];
            }
            //STRING    gridsquare
            if (adifout.ContainsKey("gridsquare"))
            {
                gridsquare = adifout["gridsquare"];
            }
            //STRING    mode
            if (adifout.ContainsKey("mode"))
            {
                mode = adifout["mode"];
            }
            //STRING    rst_sent
            if (adifout.ContainsKey("rst_sent"))
            {
                rst_sent = adifout["rst_sent"];
            }
            //STRING    rst_rcvd
            if (adifout.ContainsKey("rst_rcvd"))
            {
                rst_rcvd = adifout["rst_rcvd"];
            }
            //DATE      qso_date
            if (adifout.ContainsKey("qso_date"))
            {
                qso_date = adifout["qso_date"];
            }
            //TIME      time_on
            if (adifout.ContainsKey("time_on"))
            {
                time_on = adifout["time_on"];
            }
            //DATE      qso_date_off
            if (adifout.ContainsKey("qso_date_off"))
            {
                qso_date_off = adifout["qso_date_off"];
            }
            //TIME      time_off
            if (adifout.ContainsKey("time_off"))
            {
                time_off = adifout["time_off"];
            }
            //STRING    band
            if (adifout.ContainsKey("band"))
            {
                band = adifout["band"];
            }
            //STRING    freq
            if (adifout.ContainsKey("freq"))
            {
                freq = adifout["freq"];
            }
            //STRING    station_callsign
            if (adifout.ContainsKey("station_callsign"))
            {
                station_callsign = adifout["station_callsign"];
            }else if (adifout.ContainsKey("operator"))
            {
                station_callsign = adifout["operator"];
            }
            //STRING    my_gridsquare
            if (adifout.ContainsKey("my_gridsquare"))
            {
                my_gridsquare = adifout["my_gridsquare"];
            }
            //STRING    tx_pwr
            if (adifout.ContainsKey("tx_pwr"))
            {
                tx_pwr = new String(adifout["tx_pwr"].Where(char.IsDigit).ToArray());
                if (Convert.ToInt32(tx_pwr) >= 1 && Convert.ToInt32(tx_pwr) <= 20) //Calculate the 1 point QRP bonus
                {
                    is_qrp = 1;
                }
            }
            //STRING    comments
            if (adifout.ContainsKey("comment"))
            {
                comment = adifout["comment"];
            }
            //STRING    name
            if (adifout.ContainsKey("name"))
            {
                name = adifout["name"];
            }
            //STRING    operator_.call
            if (adifout.ContainsKey("operator"))
            {
                operator_call = adifout["operator"];
            }else if (adifout.ContainsKey("station_callsign"))
            {
                operator_call = adifout["station_callsign"];
            }
            //STRING    propmode
            if (adifout.ContainsKey("propmode"))
            {
                propmode = adifout["propmode"];
            }
            //INT       is_dx
            int is_dx = 0;
            //IN PROGRESS
            int mycall = QRZ.DCXXEntity(operator_call); ;
            int theircall = QRZ.DCXXEntity(call);
            if (theircall != mycall)
            {
                is_dx = 1;
            }
            if (theircall==999 || mycall==999)
            {
                is_dx = 0;
            }
            //INT       is_event
            int is_event = 0;
            if (call.Length <= 3)
            {
                is_event = 1;
            }

            //INT       qso_score
            //Calculate the score for this QSO
            // 1 point for the QSO
            int qso_score=1;
            //additional points for DX contact
            if (is_dx==1 && is_event!=1)
            {
                qso_score+=2; //QSO score is now 3 points
            }
            else if(is_dx!=1 && is_event==1)
            {
                qso_score+=3; //QSO score is now 4 points
            }
            if (is_qrp==1&&qso_score>1)//Calculate the QRP multiplyer for the DX or Event QSO
            {
                qso_score+=1; //QSO score for QRP DX is 4 points and 5 points for event QSO
            }

            DBinsert(call, gridsquare, mode, rst_sent, rst_rcvd, qso_date, time_on, qso_date_off, time_off, band, freq, station_callsign, my_gridsquare, tx_pwr, comment, name, operator_call, propmode, is_dx, is_event,is_qrp,qso_score);
        }

        public static void DBinsert(string call, string gridsquare, string mode, string rst_sent, string rst_rcvd, string qso_date, string time_on, string qso_date_off, string time_off, string band, string freq, string station_callsign, string my_gridsquare, string tx_pwr, string comment, string name, string operator_call, string propmode, int is_dx, int is_event,int is_qrp,int qso_score)
        {
            //ADIF Log insert Query
            string query = "INSERT IGNORE INTO ft8log VALUES (default,@call, @gridsquare, @mode, @rst_sent, @rst_rcvd, @qso_date, @time_on, @qso_date_off, @time_off, @band, @freq, @station_callsign, @my_gridsquare, @tx_pwr, @comment, @name, @operator, @propmode, @is_dx, @is_event,@is_qrp,@qso_score)";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@call", call);
            cmd.Parameters.AddWithValue("@gridsquare", gridsquare);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@rst_sent", rst_sent);
            cmd.Parameters.AddWithValue("@rst_rcvd", rst_rcvd);
            cmd.Parameters.AddWithValue("@qso_date", qso_date);
            cmd.Parameters.AddWithValue("@time_on", time_on);
            cmd.Parameters.AddWithValue("@qso_date_off", qso_date_off);
            cmd.Parameters.AddWithValue("@time_off", time_off);
            cmd.Parameters.AddWithValue("@band", band);
            cmd.Parameters.AddWithValue("@freq", freq);
            cmd.Parameters.AddWithValue("@station_callsign", station_callsign);
            cmd.Parameters.AddWithValue("@my_gridsquare", my_gridsquare);
            cmd.Parameters.AddWithValue("@tx_pwr", tx_pwr);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@operator", operator_call);
            cmd.Parameters.AddWithValue("@propmode", propmode);
            cmd.Parameters.AddWithValue("@is_dx", is_dx);
            cmd.Parameters.AddWithValue("@is_event", is_event);
            cmd.Parameters.AddWithValue("@is_qrp",is_qrp);
            cmd.Parameters.AddWithValue("@qso_score",qso_score);

            cmd.ExecuteNonQuery();
        }
        public static void DBADIFRaw(string adif)
        {
            string query = "INSERT INTO ft8adif VALUES(default,@rawadif)";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@rawadif", adif);
            cmd.ExecuteNonQuery();
        }
    }
}

