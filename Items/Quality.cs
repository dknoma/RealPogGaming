using UnityEngine;

namespace Items {
    public static class Quality {
        public enum QualityGrade {
            Common,
            Rare,
            Epic,
            Unique,
            Legendary,
            Ancient
        }

//        public static string GradeToString(QualityGrade quality) {
////            string grade;
////            switch (quality) {
////                case QualityGrade.Common:
////                    grade = "Common";
////                    break;
////                case QualityGrade.Rare:
////                    grade = "Rare";
////                    break;
////                case QualityGrade.Epic:
////                    grade = "Epic";
////                    break;
////                case QualityGrade.Unique:
////                    grade = "Unique";
////                    break;
////                case QualityGrade.Legendary:
////                    grade = "Legendary";
////                    break;
////                case QualityGrade.Ancient:
////                    grade = "Ancient";
////                    break;
////                default:
////                    return "Common";
////            }
////            return grade;
//            return quality.ToString();
//        }

        public static Color GradeToColor(QualityGrade quality) {
            Color color;
            switch (quality) {
                case QualityGrade.Common:
                    color = Color.white;
                    break;
                case QualityGrade.Rare:
                    color = new Color32(38,99,212, 255);
                    break;
                case QualityGrade.Epic:
                    color = new Color32(125,44,168, 255);
                    break;
                case QualityGrade.Unique:
                    color = new Color32(242,198,51, 255);
                    break;
                case QualityGrade.Legendary:
                    color = new Color32(81,217,13, 255);
                    break;
                case QualityGrade.Ancient:
                    color = new Color32(176,70,25, 255);
                    break;
                default:
                    color = Color.white;
                    break;
            }
            return color;
        }
    }
}
