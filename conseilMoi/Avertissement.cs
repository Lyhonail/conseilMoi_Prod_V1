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
using System.IO;
using Mono.Data.Sqlite;
using conseilMoi.Resources.Classes;
using conseilMoi.Classes;

namespace conseilMoi
{
    [Activity(Label = "Avertissement")]
    public class Avertissement : Activity
    {
        ExpandableListViewAdapter_TexteRelatif mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();

        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            MaBase db = new MaBase();
            db.ExistBase(this);
            SetContentView(Resource.Layout.Avertissement);
            MobileBarcodeScanner.Initialize(Application);


            expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewAvert);
            SetData(out mAdapter);
            expandableListView.SetAdapter(mAdapter);

            expandableListView.ChildClick += (s, e) =>
            {
                Toast.MakeText(this, "clicked: " + mAdapter.GetChild(e.GroupPosition, e.ChildPosition), ToastLength.Short).Show();

            };


            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewAvertissementProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewAvertissementHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewAvertissementScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewAvertissementConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewAvertissementAvertissement);
            var btnMajBase = FindViewById<Button>(Resource.Id.buttonMajBase);


            var txtIdProduit = FindViewById<TextView>(Resource.Id.textViewIdProduitAv);
            /*
            String test = "test or";
            test = test.Substring(0, test.Length - 2);
            txtIdProduit.Text = test;
            */


            btnMajBase.Click += delegate
            {
                String path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //chemin d'enregistrement de la base
                String maBase = Path.Combine(path, "maBase.sqlite");
                db.ReCreerBase(Resources.OpenRawResource(Resource.Raw.data), maBase);
                /*
                var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //Console.WriteLine("Data path:" + Database.DatabaseFilePath);
                var dbFile = Path.Combine(docFolder, "data4.sqlite"); // FILE NAME TO USE WHEN COPIED
                System.IO.File.Delete(dbFile);
                File.Delete(dbFile);
                // SqliteConnection.CreateFile(dbFile);

                var s = Resources.OpenRawResource(Resource.Raw.data);  // DATA FILE RESOURCE ID
                FileStream writeStream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.Write);
                ReadWriteStream(s, writeStream);
                */
            };

            Produits produits = new Produits();
            produits = db.SelectIdProduit("3250390503101", "PERS");
           // txtIdProduit.Text = produits.GetId_Produit();



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





            /* Phase test on envoie simplement un numero de produit test en attendant la version finale
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
                
            }; */
            //Clik sur le bouton scanner
            menuScanner.Click += delegate
            {

                //Intent garde la variable ID Produit et la transmet à l'activité Produit
                Intent produit = new Intent(this, typeof(Produit));
                //produit.PutExtra("IDproduit", "2000000010281");
                produit.PutExtra("IDproduit", "2000000010281");
                StartActivity(produit);
            };

        }

        private void SetData(out ExpandableListViewAdapter_TexteRelatif mAdapter)
        {
            List<string> groupA = new List<string>();
            groupA.Add(
            "1.1. Mentions légales\n" +
            "Conseil’Moi est une application développée par un groupe d’étudiant dans le cadre de leur projet de fin d’étude.\n\n"

            + "1.2.Définitions\n"
            + "Pour les besoins des présentes Conditions Générales d’Utilisation, les termes avec commençant par une majuscule sont définis comme suit:"
            + "    •	Le terme « Service » désigne ici l’ensemble des services proposés par l’application Conseil’moi tels que définis dans la rubrique 2.Description du Service."
            + "    •	Les termes « Vous », « Utilisateur » ou « Consommateur » désignent ici toute personne physique qui utilise l’application et bénéficie du Service dans le cadre d’un usage strictement privé."
            + "    •	Le terme « Société » désigne le groupement d’étudiant qui édite et exploite l’application et fournit le Service aux Utilisateurs et aux Professionnels de Santé.\n\n"

            + "1.3.Acceptation des Conditions d’Utilisation du Service\n"
            + "Les présentes Conditions d’Utilisation du Service ont pour objet de définir les conditions dans lesquels l’Utilisateur peut bénéficier du Service fourni par la Société.L’utilisation de ce Service est soumise à l’acception inconditionnelle par l’Utilisateur des Conditions Générales d’Utilisation du Service."
            + "En accédant à l’application et en bénéficiant du Service, l’Utilisateur reconnaît donc avoir lu et compris l’intégralité des présentes Conditions Générales d’Utilisation, et les accepter sans restriction ni réserves."
            + "Si l’Utilisateur n’accepte pas ces conditions, il doit cesser d’utiliser l’application et renoncer à bénéficier du Service proposé par la Société.\n\n"

            + "1.4.Champs d’application\n"
            + "Les présentes Conditions Générales d’Utilisation sont en vigueur à compter du 15 Mai 2017."
            + "Elles sont applicables à toute utilisation de l’application par l’Utilisateur.Les présentes conditions sont soumises au droit français: tout Utilisateur étranger accepte expressément l’application de la loi française en utilisant le Service."
            + "Dans l’hypothèse où une clause contractuelle particulière serait nulle, illégale, ou inapplicable, la validité des autres dispositions des Conditions Générales n’en serait aucunement affectée.\n\n"

            + "1.5.Modification des Conditions d’Utilisation du Service\n"
            + "La Société se réserve la faculté de modifier les présentes Conditions Générales d’Utilisation du Service à tout moment et sans préavis ni information, et sans préjudice.Les Conditions d’Utilisation du Service applicables sont celles en vigueur à la date et à l’heure d’activation par l’Utilisateur du bouton « J’accepte les Conditions Générale d’Utilisation » lors de l’adhésion au Service.\n\n"
            );

            List<string> groupB = new List<string>();
            groupB.Add(
            "2.1.Objectif du Service\n"
                + "L’application Conseil’Moi permet de scanner un article alimentaire et de connaitre l’adéquation de ce produit avec les critères utilisateurs."
                + "Le Service vise à faciliter la compréhension des étiquettes produits et de faire correspondre rapidement les caractéristiques du produit avec les contraintes de l’utilisateur.Par ailleurs, le Service ne constitue pas un service de recommandation d’un Professionnel de Santé.\n\n"
                + "2.2.Fonctionnement du Service\n"
                + "L’Utilisateur accède au Service via l’application Conseil’Moi."
                + "Sur l’application, l’Utilisateur peut:"
                + "· scanner un produit"
                + "· Consulter les informations composant le produit"
                + "· obtenir des recommandations si le produit scanné n’est pas totalement compatible avec les critères utilisateurs."
                + "· De consulter l’historique de ses consultations produits.\n\n"
                + "2.3.Gratuité du Service pour l’Utilisateur\n"
                + "Le Service est proposé gratuitement à l’Utilisateur.\n\n"
                + "2.4.Création d’un profil personnel\n"
                + "Pour pouvoir accéder au Service, et afin d’en assurer son bon fonctionnement, l’Utilisateur devra ouvrir un « profil personnel » lors de sa première utilisation."
                + "L'Utilisateur est seul et entièrement responsable des renseignements portés sur son profil."
                + "D’autre part, les données ainsi créées ne sont pas récupérées par l’application et ne peuvent être exploité en dehors de l’application elle - même.\n\n"
                + "2.5.Limitations du Service\n"
                + "Le référencement des produits repose sur l’utilisation d’une base de données collaborative « OpenFoodFacts » "
                + "« Open Food Facts ne garantit pas l'exactitude des informations et données présentées sur le site et dans la base de données (y compris, et sans que cette liste soit limitative, des données des produits : photos, code barre, nom, dénomination générique, quantité, conditionnement, marques, catégories, origines, labels, certifications, récompenses, codes emballeur, ingrédients, additifs, allergènes, traces, informations nutritionnelles, informations écologiques etc.)."
                + "Ces informations sont entrées par les contributeurs du site et peuvent comporter des erreurs dues par exemple à l'inexactitude des informations sur les emballages et étiquettes, à la saisie manuelle des données, ou aux traitements informatiques des données."
                + "Afin de permettre la vérification des informations par les utilisateurs, les contributeurs sont invités à envoyer des photographies des emballages et étiquettes comportant ces informations."
                + "Les utilisateurs qui relèvent des erreurs sont invités à les corriger en devenant contributeurs.Il suffit de quelques minutes pour s'inscrire en tant que contributeur et pour corriger une fiche produit. »\n\n"
                + "Complétude et exhaustivité des informations et données\n"
                + "Open Food Facts ne garantit pas la complétude et l'exhaustivité des informations et données présentées sur le site et dans la base de données."
                + "Le fait qu'un produit soit présent sur le site ou la base de données ne garantit pas que toutes les données concernant le produit y sont présentes. Les utilisateurs qui relèvent des informations manquantes sont invités à les compléter en modifiant la fiche du produit."
                + "D'autre part, tous les produits alimentaires ne sont pas présents sur Open Food Facts, compte tenu du nombre de produits alimentaires existant dans le monde entier et du nombre de nouveaux produits créés chaque jour."
                + "Les moyennes et autres informations statistiques sont calculées sur la base des produits et des données présents dans la base de données d'Open Food Facts, et non sur la totalité des produits du marché. De même les comparaisons par rapport aux moyennes, ainsi que les comparatifs de produits, sont établis sur la base des produits et données présents dans la base d'Open Food Facts.\n\n"
                + "Avertissement\n"
                + "Les informations sont fournies à titre indicatif seulement.Elles peuvent présenter des erreurs.Elles ne doivent en aucun cas être utilisées pour des raisons médicales.\n\n"
                + "Limitations de responsabilité\n"
                + "Le service est fourni en l'état. Open Food Facts ne garantit pas sa conformité à un usage particulier, ni sa compatibilité avec d'autres services tiers."
                + "De même, les informations et données sont fournies en l'état. Open Food Facts ne garantit pas leur exactitude, leur exhaustivité, leur complétude et leur conformité à un usage particulier."
                + "Le service peut être interrompu temporairement pour maintenance, ou pour des raisons indépendantes de la volonté d'Open Food Facts, notamment en cas de problèmes techniques matériels ou logiciels."
                + "L'éditeur d'Open Food Facts ne saurait être tenu comme responsable d'éventuels dommages directs ou indirects ou de perte de données liés à l'utilisation ou à l'impossibilité d'utilisation de ses services, ou à l'accès ou à l'impossibilité d'accès aux contenus des services, ou au caractère inexact, incomplet et/ou non-exhaustif des informations et données du service ou de la base de données.\n\n"
                );

            List<string> groupD = new List<string>();
            groupD.Add(
            "3.1.Conditions d’accès au service\n"
            + "Le Service est ouvert à tous mais son utilisation est subordonnée aux conditions suivantes:"
            + "•	Etre majeur"
            + "•	Reconnaitre avoir lu et compris l’intégralité des présentes Conditions Générales d’Utilisation, et les accepter sans restrictions ni réserves\n\n"
                );

            List<string> groupE = new List<string>();
            groupE.Add("Aucune données personnelles n’est demandée ni nécessaire pour l’utilisation de Conseil’Moi.");

            List<string> groupF = new List<string>();
            groupF.Add("5.1. Disponibilité du Service\n"
            + "L’application s’inscrit dans le cadre d’un projet de fin d’étude et n’est pas garantie dans le temps.La responsabilité de l’application ne peut pas être engagée en cas de dommages liés à l'impossibilité temporaire d'accéder à l'un des services proposés par le Site.\n\n"
            + "5.2.Responsabilité de l’Utilisateur\n"
            + "L'Utilisateur est seul responsable de l'utilisation qu'il fait de l’application et des renseignements inscrits dans ses profils.\n\n"
            + "5.3.Liens vers sites tiers\n"
            + "L’application peut contenir des liens hypertextes renvoyant vers des sites Internet de tiers, notamment vers des produits de substitutions et des Professionnels de Santé.\n\n"
            + "5.4.Force majeure\n"
            + "La responsabilité de l’application ne pourra pas être recherchée si l'exécution de l'une de ses obligations est empêchée ou retardée en raison d'un cas de force majeure tel que définie par la jurisprudence des Tribunaux français, et notamment les catastrophes naturelles, incendies, dysfonctionnement ou interruption du réseau de télécommunications ou du réseau électrique.\n\n"
            );

            List<string> groupG = new List<string>();
            groupG.Add("Pour toute information relative au fonctionnement du Service accessible via le Site, l'Utilisateur est invité à se reporter à la rubrique Contactez-nous.\n\n ");

            List<string> groupH = new List<string>();
            groupH.Add(
            "7.1.Protection de l’application Consei’Moi\n"
            + "La Société est le titulaire ou le concessionnaire des droits de propriété intellectuelle tant de la structure générale de l’application que de son contenu(textes, slogans, graphiques, images, photos et autres contenus)."
            + "Dès lors, conformément aux dispositions du Livre 1er du Code de la propriété intellectuelle, toute représentation, reproduction, modification, dénaturation et / ou exploitation totale ou partielle du Site, de son contenu ou du Service, par quelque procédé que ce soit et sur quelque support que ce soit, sans l'autorisation expresse et préalable de la Société, est prohibée et constitue des actes de contrefaçon de droits d'auteur.\n\n"
            + "7.2.Protection des signes distinctifs\n"
            + "Les marques, logos, dénominations sociales, sigles, noms commerciaux, enseignes et nom de domaine de la Société permettant l'accès au Service constituent des signes distinctifs insusceptibles d'utilisation sans l'autorisation expresse et préalable de leur titulaire."
            + "Toute représentation, reproduction ou exploitation partielle ou totale de ces signes distinctifs est donc prohibée et constitutif de contrefaçon de marque, en application des dispositions du Livre 7 du Code de la propriété intellectuelle, d'usurpation de dénomination sociale, nom commercial et de nom de domaine engageant la responsabilité civile délictuelle de son auteur.\n\n"
            );

            List<string> groupI = new List<string>();
            groupI.Add("La loi applicable est la loi française. En cas de litige, le Tribunal de Toulouse, France est le seul compétent.");

            List<string> groupJ = new List<string>();
            groupJ.Add("Conseil'Moi met à disposition de l'utilisateur des profils type(Sportif, Femme enceinte, ...) contenant des critères alimentaires avec des valeurs standards pour une population moyenne.Ces critères ne peuvent être modifiés directement par l'utilisateur."
            + "Pour obtenir un profil personnalisé qui prendra en compte votre age, poids, taille, activité physique, etc, vous pouvez contacter nos partenaires diététiciens nutritionnistes référencés sur l'application, qui vous délivreront votre profil personnalisé téléchargeable sur votre application Conseil'Moi.");

            group.Add("Préambule");
            group.Add("1.Disposition Générale");
            group.Add("2.Description du service");
            group.Add("3.Regle d'utilisation du service");
            group.Add("4.Protection des données personnelles");
            group.Add("5.Limitation de responsablité");
            group.Add("6.Réclamation et résiliation");
            group.Add("7.Propriété intellectuelle");
            group.Add("8.Loi applicable");

            dicMyMap.Add(group[0], groupJ);
            dicMyMap.Add(group[1], groupA);
            dicMyMap.Add(group[2], groupB);
            dicMyMap.Add(group[3], groupD);
            dicMyMap.Add(group[4], groupE);
            dicMyMap.Add(group[5], groupF);
            dicMyMap.Add(group[6], groupG);
            dicMyMap.Add(group[7], groupH);
            dicMyMap.Add(group[8], groupI);

            mAdapter = new ExpandableListViewAdapter_TexteRelatif(this, group, dicMyMap);
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}