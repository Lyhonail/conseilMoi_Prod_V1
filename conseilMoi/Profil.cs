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
using conseilMoi.Classes;
//using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Graphics;

namespace conseilMoi
{
    [Activity(Label = "Profil")]
    public class Profil : Activity
    {
       ListView lstData;
        List<Profils> lstSource = new List<Profils>();
        MaBase db = new MaBase();

        ExpandableListViewAdapter mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();

        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            db.ExistBase(this);

            SetContentView(Resource.Layout.Profil);

            // var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // SetSupportActionBar(toolbar);
            //SupportActionBar(toolbar);

            /* ANCIEN VERSION VINCENT
            expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListView);
            SetData(out mAdapter);
            expandableListView.SetAdapter(mAdapter);

            expandableListView.ChildClick += (s, e) =>
            {
                Toast.MakeText(this, "clicked: " + mAdapter.GetChild(e.GroupPosition, e.ChildPosition), ToastLength.Short).Show();

            };
           ANCIEN VERSION VINCENT  */


            //lstData = FindViewById<ListView>(Resource.Id.listViewNomProfil);
            //LoadData();
            /*lstData.ItemClick += (s, e) =>
            {
                for (int i = 0; i < lstData.Count; i++)
                {
                    if (e.Position == i)
                    {
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.DarkGray);
                    }
                    else
                    {
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }

                }

            };
            */
            MobileBarcodeScanner.Initialize(Application);

            var txtViewTitre = FindViewById<TextView>(Resource.Id.textView1);
            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewProfilProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewProfilHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewProfilScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewProfilConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewProfilAvertissement);

            Button btnProfilPERS = FindViewById<Button>(Resource.Id.buttonProfilPERS);
            Button btnProfilFAML = FindViewById<Button>(Resource.Id.buttonProfilFAML);
            Button btnProfilINVT = FindViewById<Button>(Resource.Id.buttonProfilINVT);


            menuProfil.Click += delegate
            {
                StartActivity(typeof(Profil));
            };
            menuHistorique.Click += delegate
            {
                StartActivity(typeof(Historique));
            };

            menuConseil.Click += delegate
            {
                StartActivity(typeof(Conseil));
            };

            //Clik sur le bouton scanner
            menuScanner.Click += async (sender, e) =>
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
                {
                    //Intent garde la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    produit.PutExtra("IDproduit", result.Text);
                    StartActivity(produit);
                }
                else { }
            };

            menuAvertissement.Click += delegate
            {
                StartActivity(typeof(Avertissement));
            };

            /* NOUVELLE VERSION LIONEL -- NOUVELLE VERSION LIONEL -- NOUVELLE VERSION LIONEL --   */
            btnProfilPERS.Click += delegate
            {
                MakeData("PERS");
            };
            



            /*FIN NOUVELLE VERSION LIONEL -- FIN NOUVELLE VERSION LIONEL -- FIN NOUVELLE VERSION LIONEL --   */
        }

        //ANCIENNE VERSION VINCENT
        private void SetData(out ExpandableListViewAdapter mAdapter)
        {
          

            List<Profils> groupProfils = new List<Profils>();
            groupProfils = db.SelectNomProfil();


            foreach (Profils p in groupProfils)
            {
                string pid = p.GetIdProfil();
                group.Add(p.GetNomProfil());
                List<ProfilsStandards> groupeCriteres = new List<ProfilsStandards>();
                groupeCriteres = db.SelectcritereProfilstandard(pid);

                List<string> groupC = new List<string>();
                foreach (ProfilsStandards ps in groupeCriteres)
                {
                    string valeur = ps.GetidCritere();

                    valeur += " "+db.VerifProfilUtilisateur(valeur);

                    groupC.Add(valeur);
                }
                dicMyMap.Add(p.GetNomProfil(), groupC);
            }

            mAdapter = new ExpandableListViewAdapter(this, group, dicMyMap);
        }
        // FIN ANCIENNE VERSION VINCENT--------------------------------------------------------------


       //NOUVELLE VERSION LIONEL-------------------------------------------------------------
       public void MakeData(String ID_typeProfil)
        {
            //Chargement du linearLayout à remplir
            LinearLayout linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutProfile);

            //je déclare la liste des groupes de profil
            List<Profils> groupProfils = new List<Profils>();
            groupProfils = db.SelectNomProfil();

            //pour caque groupe de prifil, on l'afficher et chercher ses sous-groupes
            foreach (Profils p in groupProfils)
            {
                //JE créer le TextView
                TextView textView1 = new TextView(this) { Id = 1 };
                //Je rempli le texte
                textView1.Text = p.GetNomProfil();
                //Je définis la couleur à "Black" car c'est blanc par défaut
                textView1.SetTextColor(Color.Black);
                //Je définis les paramètres du textView
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                //J'ajoute le TextView tout bien rempli à mon LinearLayout
                linearLayout.AddView(textView1, param);

                //Je déclare la liste des sous-groupes
                List<ProfilsStandards> groupeCriteres = new List<ProfilsStandards>();
                //Je rempli la liste des sous-groupes avec la BDD
                groupeCriteres = db.SelectcritereProfilstandard(p.GetIdProfil());

                LinearLayout LN = new LinearLayout(this) { Id = 5 };
                var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                LN.Orientation = Orientation.Vertical;
                linearLayout.AddView(LN);
                //variable pour vérifier si le groupe est ouvert ou fermé
                bool ouvert = false;

                textView1.Click += delegate
                {
                    //Si le groupe est fermé, on va l'ouvrir
                    if (ouvert == false)
                    {
                        ouvert = true;
                        //Pour chaque sous-goupe trouvé, on va l'afficher !
                        foreach (ProfilsStandards ps in groupeCriteres)
                        {
                           
                            //Je créer la CheckBox
                            CheckBox checkBox = new CheckBox(this) { Id = 10 };
                            checkBox.Text = ps.GetidCritere();
                            checkBox.SetTextColor(Color.Black);
                            var param2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            LN.AddView(checkBox, param);
                            //Je check la checkBox, ou pas
                            String res = db.VerifProfilUtilisateur(ps.GetidCritere());
                            string[] words = res.Split(' ');
                            if (words[0] == "check") { checkBox.Checked = true; }

                            checkBox.Click += delegate
                            {
                                if (checkBox.Checked == true) { db.InsertProfilUtilisateur(ps.GetidCritere(), ID_typeProfil); }
                                if (checkBox.Checked == false) { db.DeleteProfilUtilisateur(ID_typeProfil, p.GetNomProfil(), ps.GetidCritere()); }
                            };

                        }
                    } //FIN SI
                    //Sinon, s'il est ouvert, on le ferme
                    else
                    {
                        ouvert = false;
                        LN.RemoveAllViews();
                    } //FIN SINON
                }; //FIN textView1.Click
            }


            /*
                for (int a = 0; a < 5; a++)
            {
                TextView textView1 = new TextView(this) { Id = 1 };
                textView1.Text = "GROUPE";
                textView1.SetTextColor(Color.Black);
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                // param.AddRule(LayoutRules.AlignParentTop);
                linearLayout.AddView(textView1, param);

                LinearLayout LN = new LinearLayout(this) { Id = 5 };
                var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                LN.Orientation = Orientation.Vertical;

                linearLayout.AddView(LN);


                textView1.Click += delegate
                {

                    if (textView1.Text == "GROUPE")
                    {

                        textView1.Text += " ouvert";
                        for (int b = 0; b < 3; b++)
                        {
                            TextView textView2 = new TextView(this) { Id = 10 };
                            textView2.SetTextColor(Color.Black);
                            textView2.Text = "ITEM ";
                            var param2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            LN.AddView(textView2, param);
                        }

                    }
                    else
                    {
                        LN.RemoveAllViews();
                        textView1.Text = "GROUPE";

                    }


                };
                

            }
        */
        }

       //FIN NOUVELLE VERSION LIONEL--------------------------------------------------------------

        private void LoadData()
        {
            lstSource = db.SelectNomProfil();
            var adapter = new ListViewAdapterProfil(this, lstSource);
            lstData.Adapter = adapter;
        }

    }
}