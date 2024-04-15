using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace C_App6
{
    public partial class RssReader : Form
    {

        List<string> listLink = new List<string>();
        public RssReader()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Rssファイル読み込み
            //ウェブクライエントで開いて、
            //ストリームとして読み込む
            //RssAll();

            //xmlReaderで
            OpenRssXmlReader();

            //OpenRssDom();
            //OpenRssLINQ();
        }

        private void RssAll()
        {
            //ネットアクセスするためのwebClient()
            WebClient wc = new WebClient();

            //ストリームとして読み込む
            using (Stream stream = wc.OpenRead(textBoxURL.Text))
            {
                //ストリームリーダーを作成（UTF-8でエンコード）
                using (StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                {
                    //ストリームリーダー
                    while (!sr.EndOfStream)
                    {
                        //リストボックスに１行ずつ追加
                        listBoxRss.Items.Add(sr.ReadLine());
                    }
                }
            }
        }

        private void OpenRssXmlReader()
        {
            try
            {
                using (XmlReader xR = XmlReader.Create(textBoxURL.Text))
                {

                    //ノードがなくなるまで読み込む
                    while (xR.Read())
                    {

                        //itemへ移動
                        xR.ReadToFollowing("item");
                        //titleへ移動
                        xR.ReadToFollowing("title");
                        //<title>の中身をリストボックスに表示
                        listBoxRss.Items.Add(xR.ReadString());

                        //リンクへ移動
                        xR.ReadToFollowing("link");

                        //リンクリストに中身を追加
                        listLink.Add(xR.ReadString());

                    }
                    //スクリプトエラー回避
                    webBrowserRss.ScriptErrorsSuppressed = true;
                }

            }
            catch(Exception ex)
            {
                //エラー表示
                Console.WriteLine(ex.ToString());
                MessageBox.Show("正しい数値を入力してください", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }




        private void OpenRssDom()
        {

            try
            {

                //Dom走査を行う宣言
                XmlDocument xD = new XmlDocument();
                //RSSファイル読み込み
                xD.Load(textBoxURL.Text);

                //全てのitemノード走査
                foreach (XmlNode itemNode in xD.GetElementsByTagName("item"))
                {

                    //itemNodeのノードの、子ノード全て走査
                    foreach (XmlNode node in itemNode.ChildNodes)
                    {
                        //nodeの名前がタイトル
                        if (node.Name == "title")
                        {
                            //<titile></title>の中身を
                            //LsitBoxRssに表示する
                            listBoxRss.Items.Add((string)node.InnerText);

                        }

                        //リンクリストに中身を追加
                        if (node.Name == "link")
                        {
                            listLink.Add((string)node.InnerText);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                //エラー表示
                Console.WriteLine(ex.ToString());
                MessageBox.Show("正しい数値を入力してください", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OpenRssLINQ()
        {

            try
            {

                //URLからRSSファイルを読み取る
                XElement xE = XElement.Load(textBoxURL.Text);

                //クエリ要素内のtitleとlink要素の値を取り出す
                var query = from item in xE.Descendants("item")
                            select new
                            {
                            title = item.Element("title").Value,
                            link = item.Element("link").Value
                            };


                foreach (var item in query)
                {
                    //<title>要素をリストボックスに追加
                    listBoxRss.Items.Add(item.title);

                    //listLink にLink要素 URL を追加
                    listLink.Add(item.link);
                }
                
            }
            catch (Exception ex)
            {
                //エラー表示
                Console.WriteLine(ex.ToString());
                MessageBox.Show("正しい数値を入力してください","入力エラー",MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void listBoxRss_SelectedIndexChanged(object sender, EventArgs e)
        {
            //listLink[i] と listBoxのIndex連動
            //リンク先のwebページを表示
            webBrowserRss.Navigate(listLink[listBoxRss.SelectedIndex]);

            //スクリプトエラー回避
            webBrowserRss.ScriptErrorsSuppressed = true;
        }



        private void comboBoxRss_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxURL.Text = comboBoxRss.SelectedItem.ToString();

        }



        private void registButton_Click(object sender, EventArgs e)
        {
            if (comboBoxRss.Text != "")
            {
                comboBoxRss.Items.Add(comboBoxRss.Text);
                textBoxURL.Text = comboBoxRss.Text;
                comboBoxRss.Text = "";
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (comboBoxRss.Items.Count!=0)
            {
                if (comboBoxRss.SelectedIndex != -1)
                {
                    comboBoxRss.Items.RemoveAt(comboBoxRss.SelectedIndex);
                    textBoxURL.Text = "";
                    comboBoxRss.Text = "";
                }

            }    
        }
    }
}
