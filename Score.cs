using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Nez;

namespace MarshmallowAvalanche {
    public class Score {
        public float TopScore { get; set; }

        [NonSerialized]
        public float CurrentScore;
        private float startingHeight;

        public void SetStartingHeight(float position) {
            startingHeight = position;
            CurrentScore = 0;
        }

        public void UpdateScoreIfBetter(float newPos) {
            CurrentScore = Math.Max(CurrentScore, startingHeight - newPos);
            TopScore = Math.Max(TopScore, CurrentScore);
        }


        #region Serialization
        private const string dirname = "MarshmallowAvalanche";
        private const string filename = "data.xml";

        /// <summary>
        /// Loads the saved game data. If no data exists, a new
        /// Score object will be instantiated and returned.
        /// </summary>
        /// <returns></returns>
        public static Score LoadData() {
            try {
                string dir = GetAppDir();
                dir = Path.Combine(dir, dirname);
                if (!Directory.Exists(dir)) {
                    return new Score()
                    {
                        TopScore = 0
                    };
                }

                string filepath = Path.Combine(dir, filename);
                if (!File.Exists(filepath)) {
                    return new Score()
                    {
                        TopScore = 0
                    };
                }

                Score deserializedScore;
                XmlSerializer serializer = new XmlSerializer(typeof(Score));

                using (FileStream fs = File.OpenRead(filepath)) {
                    deserializedScore = (Score) serializer.Deserialize(fs);
                }

                return deserializedScore;
            }
            catch (Exception e) {
                Debug.Error("Exception while loading data: " + e.Message);
                return new Score()
                {
                    TopScore = 0
                };
            }
        }

        public static void ResetTopScore() {
            Score score = new Score()
            {
                TopScore = 0
            };
            score.SaveData();
        }

        public void SaveData() {
            try {
                string dir = GetAppDir();
                dir = Path.Combine(dir, dirname);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                XmlDocument doc = new XmlDocument();
                XmlElement topScoreElement = doc.CreateElement("TopScore");
                topScoreElement.InnerText = TopScore.ToString();

                string filepath = Path.Combine(dir, filename);

                XmlSerializer serializer = new XmlSerializer(typeof(Score));
                using StreamWriter sw = new StreamWriter(filepath);
                serializer.Serialize(sw, this);
            }
            catch (Exception e) {
                Debug.Error("Unable to save data: " + e.Message);
            }
        }

        private static string GetAppDir() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        #endregion
    }
}
