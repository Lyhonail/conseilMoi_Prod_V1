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
    class ProfilsStandards
    {
        String idprofil;
        String idcritere;
        String valeur;
        String seuilV;
        String seuilO;
        String seuilR;
       // Boolean selected;

        public void CreeProfilStandard(String ip, String ic, String v, String sv, String so, String sr)
        {
            idprofil = ip;
            idcritere = ic;
            valeur = v;
            seuilV = sv;
            seuilO = so;
            seuilR = sr;
            //selected = false;
        }

        public String GetidProfil()
        {
            return idprofil;
        }
        public String GetidCritere()
        {
            return idcritere;
        }
        public String GetValeur()
        {
            try
            {
                if (valeur == "")
                {
                    valeur = "Vide";
                    return valeur;
                }
                return valeur;

            }
            catch
            {
                return seuilR;
            }
        }
        public String GetseuilV()
        {
            return seuilV;
        }
        public String GetseuilO()
        {
            return seuilO;
        }
        public String GetseuilR()
        {
            try
            {
                if (seuilR == "")
                {
                    seuilR = "Vide";
                    return seuilR;
                }
                return seuilR;

            }
            catch
            {
                return seuilR;
            }
            
           
        }

    }
}