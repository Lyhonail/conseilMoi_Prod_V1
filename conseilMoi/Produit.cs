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

namespace conseilMoi
{
    [Activity(Label = "Produit")]
    public class Produit : Activity
    {
        ListView lstData;
        List<Produits> lstSource = new List<Produits>();
        MaBase db1 = new MaBase();

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


            //Fait un enregistrement dans historique
            txtInfoScan.Text = db.InsertIntoHistorique(IDTypeProfil, IDproduit);

            //On charge le produit Le IdProduit va dans le texteView IdProduit 
            //On créer d'abbord un objet produit qui contiendra tout le contenu du produit
            Produits produits = new Produits();
            produits = db.SelectIdProduit(IDproduit, IDTypeProfil);

            txtIdProduit.Text = "Id : " + produits.GetId_Produit() + ", Nom : " + produits.GetProduct_name();
            txtInfoScan.Text += "Code scanné : " + IDproduit;
            //txtInfoAllergene.Text = produits.GetAllergenes();
            //txtInfoAllergene.Text = produits.GetNutriments();

            /* VERIFIE LES ALLERGENES */
            List<Allergene> ListAl = new List<Allergene>();
            ListAl = produits.GetCheckAllergene();

            try
            {
                if (ListAl[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; }
            }

            catch
            {
                txtInfoAllergene.Text = "pas d'allergene";
            }
            /* FIN VERIFIE LES ALLERGENES */

            /* VERIFIE LES NUTRIMENTS  */
            List<Nutriment> ListNut = new List<Nutriment>();
            ListNut = produits.GetCheckNutriment();

            try
            {
                if (ListNut[0].GetIdNutriment() == "") { txtInfoNutriment.Text = "pas d'allergene"; }
                else
                {
                    txtInfoNutriment.Text = ListNut[0].GetIdTypeProfil() + " " + ListNut[0].GetIdProfil() + " " + ListNut[0].GetIdNutriment() + " " +
                                             ListNut[0].GetValeurProfil() + " " + ListNut[0].GetValeurProduit() + " " +
                                             ListNut[0].GetVert() + " " + ListNut[0].GetOrange() + " " + ListNut[0].GetRouge();
                }
            }

            catch
            {
                txtInfoNutriment.Text = "pas de nutriment trouvé";
            }
            /* FIN VERIFIE LES NUTRIMENTS   */


            //-----------------/* BOUTON CHOIX PROFIL PERSO */ //-----------------------------------//
            btnProduitPERS.Click += delegate
            {
                IDTypeProfil = "PERS";
                Produits produitPerso = new Produits();

                produitPerso = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergenePerso = new List<Allergene>();
                ListAllergenePerso = produitPerso.GetCheckAllergene();

                try
                {
                    if (ListAllergenePerso[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentPerso = new List<Nutriment>();
                ListNuttrimentPerso = produitPerso.GetCheckNutriment();

                try
                {
                    if (ListNuttrimentPerso[0].GetIdNutriment() == "") { txtInfoNutriment.Text = "pas d'allergene"; }
                    else
                    {
                        txtInfoNutriment.Text = ListNuttrimentPerso[0].GetIdTypeProfil() + " " + ListNuttrimentPerso[0].GetIdProfil() + " " + ListNuttrimentPerso[0].GetIdNutriment() + " " +
                                                   ListNuttrimentPerso[0].GetValeurProfil() + " " + ListNuttrimentPerso[0].GetValeurProduit() + " " +
                                                   ListNuttrimentPerso[0].GetVert() + " " + ListNuttrimentPerso[0].GetOrange() + " " + ListNuttrimentPerso[0].GetRouge();
                    }
                }

                catch { txtInfoNutriment.Text = "pas de nutriment trouvé"; }
                /* FIN VERIFIE LES NUTRIMENTS   */
            };
            /* FIN  BOUTON CHOIX PROFIL PERSO   */

            btnProduitFAM.Click += delegate
            {
                txtInfoNutriment.Text = "Clik sur famille";
                IDTypeProfil = "FAML";
                Produits produitFamille = new Produits();
                produitFamille = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneFamille = new List<Allergene>();
                ListAllergeneFamille = produitFamille.GetCheckAllergene();

                try
                {
                    if (ListAllergeneFamille[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentFamille = new List<Nutriment>();
                ListNuttrimentFamille = produitFamille.GetCheckNutriment();

                try
                {
                    if (ListNuttrimentFamille[0].GetIdNutriment() == "") { txtInfoNutriment.Text = "pas d'allergene"; }
                    else
                    {
                        txtInfoNutriment.Text = ListNuttrimentFamille[0].GetIdTypeProfil() + " " + ListNuttrimentFamille[0].GetIdProfil() + " " + ListNuttrimentFamille[0].GetIdNutriment() + " " +
                                                   ListNuttrimentFamille[0].GetValeurProfil() + " " + ListNuttrimentFamille[0].GetValeurProduit() + " " +
                                                   ListNuttrimentFamille[0].GetVert() + " " + ListNuttrimentFamille[0].GetOrange() + " " + ListNuttrimentFamille[0].GetRouge();
                    }
                }

                catch { txtInfoNutriment.Text = "pas de nutriment trouvé"; }
                /* FIN VERIFIE LES NUTRIMENTS   */

            };

            btnProduitINV.Click += delegate
            {
                 IDTypeProfil = "INVT";
                Produits produitInvite = new Produits();
                produitInvite = db.SelectIdProduit(IDproduit, IDTypeProfil);

                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneInvite = new List<Allergene>();
                ListAllergeneInvite = produitInvite.GetCheckAllergene();

                try
                {
                    if (ListAllergeneInvite[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "pas d'allergene"; }
                    else { txtInfoAllergene.Text = "contient allergene correspondant a votre profil !"; }
                }
                catch { txtInfoAllergene.Text = "pas d'allergene"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentInvite = new List<Nutriment>();
                ListNuttrimentInvite = produitInvite.GetCheckNutriment();

                try
                {
                    if (ListNuttrimentInvite[0].GetIdNutriment() == "") { txtInfoNutriment.Text = "pas d'allergene"; }
                    else
                    {
                        txtInfoNutriment.Text = ListNuttrimentInvite[0].GetIdTypeProfil() + " " + ListNuttrimentInvite[0].GetIdProfil() + " " + ListNuttrimentInvite[0].GetIdNutriment() + " " +
                                                   ListNuttrimentInvite[0].GetValeurProfil() + " " + ListNuttrimentInvite[0].GetValeurProduit() + " " +
                                                   ListNuttrimentInvite[0].GetVert() + " " + ListNuttrimentInvite[0].GetOrange() + " " + ListNuttrimentInvite[0].GetRouge();
                    }
                }

                catch { txtInfoNutriment.Text = "pas de nutriment trouvé"; }
                /* FIN VERIFIE LES NUTRIMENTS   */


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

        private void LoadData()
        {
            lstSource = db1.SelectProduitRecommande();
            var adapter = new ListViewAdapterProduitRecommandation(this, lstSource);
            lstData.Adapter = adapter;
        }
        public void Afficher()
        {


        }

       

    }
}