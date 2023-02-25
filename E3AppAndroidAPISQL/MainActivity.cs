using Java.IO;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace E3AppAndroidAPISQL
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        EditText? txtNombre, txtDomicilio, txtCorreo, txtEdad, txtSaldo, txtID;
        Button? btnAlmacenar, btnConsultar, btnAlmacenarXML, btnConsultarXML;
        string? Nombre, Domicilio, Correo;
        int Edad, ID;
        double Saldo;
        HttpClient cliente = new HttpClient();
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txtID = FindViewById<EditText>(Resource.Id.txtid);
            txtNombre = FindViewById<EditText>(Resource.Id.txtnombre);
            txtDomicilio = FindViewById<EditText>(Resource.Id.txtdomicilio);
            txtCorreo = FindViewById<EditText>(Resource.Id.txtcorreo);
            txtEdad = FindViewById<EditText>(Resource.Id.txtedad);
            txtSaldo = FindViewById<EditText>(Resource.Id.txtsaldo);
            btnAlmacenar = FindViewById<Button>(Resource.Id.btnguardarSQLServer);
            btnConsultar = FindViewById<Button>(Resource.Id.btnBuscarSQLServer);
            btnAlmacenarXML = FindViewById<Button>(Resource.Id.btnguardar);
            btnConsultarXML = FindViewById<Button>(Resource.Id.btnbuscar);
            btnAlmacenarXML.Click += delegate
            {
                var DC = new Datos();
                try
                {
                    DC.Id = int.Parse(txtID.Text);
                    DC.Nombre = txtNombre.Text;
                    DC.Domicilio = txtDomicilio.Text;
                    DC.Correo = txtCorreo.Text;
                    DC.Edad = int.Parse(txtEdad.Text);
                    DC.Saldo = double.Parse(txtSaldo.Text);
                    var serializador = new XmlSerializer(typeof(Datos));
                    var Escritura = new StreamWriter
                        (Path.Combine(System.Environment.GetFolderPath
                            (System.Environment.SpecialFolder.Personal),
                                txtID.Text + ".xml"));
                    serializador.Serialize(Escritura, DC);
                    Escritura.Close();
                    txtID.Text = "";
                    txtNombre.Text = "";
                    txtDomicilio.Text = "";
                    txtCorreo.Text = "";
                    txtEdad.Text = "";
                    txtSaldo.Text = "";
                    Toast.MakeText(this, "Archivo XML Guardado Correctamente",
                        ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message,
                        ToastLength.Long).Show();
                }
            };
            btnConsultarXML.Click += delegate
            {
                var DC = new Datos();
                try
                {
                    DC.Id = int.Parse(txtID.Text);
                    var serializador = new XmlSerializer(typeof(Datos));
                    var Lectura = new StreamReader
                        (Path.Combine(System.Environment.GetFolderPath
                            (System.Environment.SpecialFolder.Personal),
                                txtID.Text + ".xml"));
                    var datos = (Datos)serializador.Deserialize(Lectura);   
                    Lectura.Close();
                    txtNombre.Text = datos.Nombre;
                    txtDomicilio.Text = datos.Domicilio;
                    txtCorreo.Text = datos.Correo;
                    txtEdad.Text = datos.Edad.ToString();
                    txtSaldo.Text = datos.Saldo.ToString();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message,
                        ToastLength.Long).Show();
                }
            };
            btnAlmacenar.Click += delegate
            {
                try
                {
                    Nombre = txtNombre.Text;
                    Domicilio = txtDomicilio.Text;
                    Correo = txtCorreo.Text;
                    Edad = int.Parse(txtEdad.Text);
                    Saldo = double.Parse(txtSaldo.Text);
                    var API = "http://172.20.10.7:85/Principal/AlmacenarSQLServer?Nombre=" +
                    Nombre + "&Domicilio=" + Domicilio + "&Correo=" + Correo +
                    "&Edad=" + Edad + "&Saldo=" + Saldo;
                    HttpResponseMessage response = cliente.GetAsync(API).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = response.Content.ReadAsStringAsync().Result;
                        Toast.MakeText(this, resultado.ToString(), ToastLength.Long).Show();
                    }
                    txtNombre.Text = "";
                    txtDomicilio.Text = "";
                    txtCorreo.Text = "";
                    txtEdad.Text = "";
                    txtSaldo.Text = "";

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message,
                        ToastLength.Long).Show();
                }
            };
            btnConsultar.Click += async delegate
            {
                try
                {
                    ID = int.Parse(txtID.Text);
                    var API = "http://172.20.10.7:85/Principal/ConsultarSQLServer?ID=" + ID;
                    var json = await TraerDatos(API);
                    foreach (var repo in json)
                    {
                        txtNombre.Text = repo.Nombre;
                        txtDomicilio.Text = repo.Domicilio;
                        txtCorreo.Text = repo.Correo;
                        txtEdad.Text = repo.Edad.ToString();
                        txtSaldo.Text = repo.Saldo.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message,
                       ToastLength.Long).Show();
                }
            };
        }
        private async Task<List<Datos>> TraerDatos(string API)
        {
            cliente.DefaultRequestHeaders.Accept.Clear();
            var streamTask = cliente.GetStreamAsync(API);
            var repositorio = await
                System.Text.Json.JsonSerializer.DeserializeAsync<List<Datos>>(await streamTask);
            return repositorio;
        }
    }
    public class Datos
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("domicilio")]
        public string Domicilio { get; set; }
        [JsonPropertyName("correo")]
        public string Correo { get; set; }
        [JsonPropertyName("edad")]
        public int Edad { get; set; }
        [JsonPropertyName("saldo")]
        public double Saldo { get; set; }

    }
}