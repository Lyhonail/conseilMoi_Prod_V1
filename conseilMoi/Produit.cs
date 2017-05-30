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
using ZXing.Mobile;
using conseilMoi.Resources.MaBase;
using conseilMoi.Resources.Classes;
using conseilMoi.Classes;
using System.Net;
using Android.Graphics;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;

namespace conseilMoi
{
    [Activity(Label = "Produit")]
    public class Produit : Activity
    {
        ListView lstData;
        List<Produits> lstSource = new List<Produits>();
        MaBase db1 = new MaBase();
        int feu = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Produit);

            //initialise la classe MaBase et connecte la base de donnnées
            MaBase db = new MaBase();
            db.ExistBase(this);
            db1.ExistBase(this);


            //initialise le scanner de code barre
            MobileBarcodeScanner.Initialize(Application);

            //Récupère le ID_Produit lorsque l'on scanne un produit
            string IDproduit = Intent.GetStringExtra("IDproduit") ?? "Data not available";
            string IDTypeProfil = "PERS";




            //chargement des variables des boutons et textViews
            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewProduitProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewProduitHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewProduitScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewProduitConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewProduitAvertissement);
            var txtIdProduit = FindViewById<TextView>(Resource.Id.textViewIdProduit);
            var txtInfoScan = FindViewById<TextView>(Resource.Id.textViewInfoScan);
            var txtInfoAllergene = FindViewById<TextView>(Resource.Id.textViewInfoAlergene);
            var txtInfoNutriment = FindViewById<TextView>(Resource.Id.textViewInfoNutriment);
            var btnProduitPERS = FindViewById<Button>(Resource.Id.buttonProduitPERS);
            var btnProduitFAM = FindViewById<Button>(Resource.Id.buttonProduitFAM);
            var btnProduitINV = FindViewById<Button>(Resource.Id.buttonProduitINV);
           ImageView imgProduit = FindViewById<ImageView>(Resource.Id.imageViewProduit);
            ImageView imgFeu = FindViewById<ImageView>(Resource.Id.imageViewFeu);
            TextView txtNutrimentList = FindViewById<TextView>(Resource.Id.textViewInfoNutrimentList);
            ListView listNutriment = FindViewById<ListView>(Resource.Id.listViewNutriment);

           

            btnProduitPERS.SetBackgroundColor(Color.LightGray);
            btnProduitFAM.SetBackgroundColor(Color.Gray);
            btnProduitINV.SetBackgroundColor(Color.Gray);
           


            txtNutrimentList.Text = "";
            

            btnProduitPERS.SetTextColor(Color.Gray);
            btnProduitFAM.SetTextColor(Color.LightGray);
            btnProduitINV.SetTextColor(Color.LightGray);

            //CHARGEMENT DE L'IMAGE
            Koush.UrlImageViewHelper.SetUrlDrawable(imgProduit, "http://fr.openfoodfacts.org/images/products/200/000/001/0281/front.4.200.jpg");

            //Fait un enregistrement dans historique
            txtInfoScan.Text = db.InsertIntoHistorique(IDTypeProfil, IDproduit);

            //On charge le produit Le IdProduit va dans le texteView IdProduit 
            //On créer d'abbord un objet produit qui contiendra tout le contenu du produit
            Produits produits = new Produits();
            produits = db.SelectIdProduit(IDproduit, IDTypeProfil);

            txtIdProduit.Text = /* "Id : " + produits.GetId_Produit() + ", Nom : " + */ produits.GetProduct_name();
            txtInfoScan.Text += "Code barre : " + IDproduit;

            /* VERIFICATION POUR LE CHARGEMENT PAR DEFAUT SUR LE PROFIL PERSO */
            /* VERIFIE LES ALLERGENES */
            List<Allergene> ListAl = new List<Allergene>();
            ListAl = produits.GetCheckAllergene();

            try
            {
                if (ListAl[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "Pas d'allergene";  }
                else { txtInfoAllergene.Text = "Allergene incompatible !"; feu = 2;  }
            }

            catch
            {
                txtInfoAllergene.Text = "Pas d'allergene"; 
            }
            /* FIN VERIFIE LES ALLERGENES */

            /* VERIFIE LES NUTRIMENTS  */
            List<Nutriment> ListNut = new List<Nutriment>();
            ListNut = produits.GetCheckNutriment();

            foreach (Nutriment n in ListNut)
            {
                try
                {
                    if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                    else
                    {
                        decimal valeur_produit = n.GetValeurProduit();
                        decimal valeur_profil = n.GetValeurProfil();
                        decimal seuil_vert = n.GetVert();
                        decimal seuil_orange = n.GetOrange();
                        decimal seuil_rouge = n.GetRouge();

                        decimal taux = valeur_profil / valeur_produit;
                        txtNutrimentList.Text += n.GetIdNutriment()+" ";

                        if (taux <= seuil_vert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible en faible quantité "; }
                        if (taux > seuil_vert && taux <= seuil_orange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité "; }
                        if (taux > seuil_orange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité"; }
                    }
                }

                catch
                {
                    txtInfoNutriment.Text = "Nutriments compatibles";
                }


            }

          

            if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
            if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
            if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

            /* FIN VERIFIE LES NUTRIMENTS */
            /* FIN DE LA VERIFICATION POUR LE CHARGEMENT PAR DEFAUT SUR LE PROFIL PERSO */


            //-----------------/* BOUTON CHOIX PROFIL PERSO */ //-----------------------------------//
            btnProduitPERS.Click += delegate
            {

                btnProduitPERS.SetBackgroundColor(Color.LightGray);
                btnProduitFAM.SetBackgroundColor(Color.Gray);
                btnProduitINV.SetBackgroundColor(Color.Gray);

                btnProduitPERS.SetTextColor(Color.Gray);
                btnProduitFAM.SetTextColor(Color.LightGray);
                btnProduitINV.SetTextColor(Color.LightGray);
                txtNutrimentList.Text = "";

                feu = 0;
                IDTypeProfil = "PERS";
                Produits produitPerso = new Produits();

                produitPerso = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergenePerso = new List<Allergene>();
                ListAllergenePerso = produitPerso.GetCheckAllergene();

                try
                {
                    if (ListAllergenePerso[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; feu = 2; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentPerso = new List<Nutriment>();
                ListNuttrimentPerso = produitPerso.GetCheckNutriment();

                foreach (Nutriment n in ListNuttrimentPerso)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();
                            decimal seuil_rouge = n.GetRouge();

                            decimal taux = valeur_profil / valeur_produit;
                            txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (taux <= seuil_vert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible en faible quantité " + taux; }
                            if (taux > seuil_vert && taux <= seuil_orange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité " + taux; }
                            if (taux > seuil_orange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité tx:" + taux + " Sv:" + seuil_vert + " So:" + seuil_orange; }
                        }
                    }

                    catch
                    {
                        txtInfoNutriment.Text = "Nutriments compatibles";
                    }
                }

                
                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }
            };
            /* FIN  BOUTON CHOIX PROFIL PERSO   */

            btnProduitFAM.Click += delegate
            {
                btnProduitPERS.SetBackgroundColor(Color.Gray);
                btnProduitFAM.SetBackgroundColor(Color.LightGray);
                btnProduitINV.SetBackgroundColor(Color.Gray);

                btnProduitPERS.SetTextColor(Color.LightGray);
                btnProduitFAM.SetTextColor(Color.Gray);
                btnProduitINV.SetTextColor(Color.LightGray);
                txtNutrimentList.Text = "";

                feu = 0;
                IDTypeProfil = "FAML";
                Produits produitFamille = new Produits();

                produitFamille = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneFamille = new List<Allergene>();
                ListAllergeneFamille = produitFamille.GetCheckAllergene();

                try
                {
                    if (ListAllergeneFamille[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; feu = 2; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentFamille = new List<Nutriment>();
                ListNuttrimentFamille = produitFamille.GetCheckNutriment();

                foreach (Nutriment n in ListNuttrimentFamille)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();
                            decimal seuil_rouge = n.GetRouge();

                            decimal taux = valeur_profil / valeur_produit;
                            txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (taux <= seuil_vert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible en faible quantité " + taux; }
                            if (taux > seuil_vert && taux <= seuil_orange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité " + taux; }
                            if (taux > seuil_orange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité tx:" + taux + " Sv:" + seuil_vert + " So:" + seuil_orange; }
                        }
                    }

                    catch
                    {
                        txtInfoNutriment.Text = "Nutriments compatibles";
                    }
                }
                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

            };

            btnProduitINV.Click += delegate
            {
                btnProduitPERS.SetBackgroundColor(Color.Gray);
                btnProduitFAM.SetBackgroundColor(Color.Gray);
                btnProduitINV.SetBackgroundColor(Color.LightGray);

                btnProduitPERS.SetTextColor(Color.LightGray);
                btnProduitFAM.SetTextColor(Color.LightGray);
                btnProduitINV.SetTextColor(Color.Gray);
                txtNutrimentList.Text = "";

                feu = 0;
                IDTypeProfil = "INVT";
                Produits produitInvite = new Produits();

                produitInvite = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneInvite = new List<Allergene>();
                ListAllergeneInvite = produitInvite.GetCheckAllergene();

                try
                {
                    if (ListAllergeneInvite[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; feu = 2; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentInvite = new List<Nutriment>();
                ListNuttrimentInvite = produitInvite.GetCheckNutriment();

                foreach (Nutriment n in ListNuttrimentInvite)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();
                            decimal seuil_rouge = n.GetRouge();

                            decimal taux = valeur_profil / valeur_produit;
                            txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (taux <= seuil_vert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible en faible quantité " + taux; }
                            if (taux > seuil_vert && taux <= seuil_orange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité " + taux; }
                            if (taux > seuil_orange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité tx:" + taux + " Sv:" + seuil_vert + " So:" + seuil_orange; }
                        }
                    }

                    catch
                    {
                        txtInfoNutriment.Text = "Nutriments compatibles";
                    }
                }
                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }
            };




            /*  MENU DU BAS  */

            //Lorsque l'on clique sur le bouton menuProfil
            menuProfil.Click += delegate
            {
                StartActivity(typeof(Profil));
            };

            //Lorsque l'on clique sur le bouton menuHistorique
            menuHistorique.Click += delegate
            {
                StartActivity(typeof(Historique));
            };


            //Lorsque l'on clique sur le bouton menuConseilMoi
            menuConseil.Click += delegate
            {
                StartActivity(typeof(Conseil));
            };

            //Clik sur le bouton scanner
            menuScanner.Click += async (sender, e) =>
            {
                //on active le lecteur code barre et on attend une réponse (un code barre est lu)
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();

                //un code bare est lu, le résultat va dans la variable résult
                var result = await scanner.Scan();
                if (result != null)
                {
                    //Intent stocke la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    produit.PutExtra("IDproduit", result.Text);
                    StartActivity(produit);
                }
                else { }
            };

            //Lorsque l'on clique sur menuAvertissement
            menuAvertissement.Click += delegate
            {
                StartActivity(typeof(Avertissement));
            };
        }

        //VINCENT -> A expliquer
        private void LoadData()
        {
            lstSource = db1.SelectProduitRecommande();
            var adapter = new ListViewAdapterProduitRecommandation(this, lstSource);
            lstData.Adapter = adapter;
        }
        



    }
}