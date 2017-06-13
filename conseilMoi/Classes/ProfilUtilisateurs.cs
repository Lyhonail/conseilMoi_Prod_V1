using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace conseilMoi.Classes
{


    class ProfilUtilisateurs
    {
        String id_critere;
        String id_valeur;

        public void CreeProfil(String np, String ip)
        {
            id_critere = np;
            id_valeur = ip;
        }

        public String GetidCritere()
        {
            return id_critere;
        }
        public String GetidValeur()
        {
            return id_valeur;
        }
    }


}