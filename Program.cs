using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TEST
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region VIEW EDITOR FOR ADDING SITECORE ERROR MESSAGES
            //DESCRIPTION: this can be used as a standalone application from this solution
            int counter = 0;
            string line;

            // Read the file line by line and add error messages for experience editor 
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"C:\Solutions\TYG.RFV\Web\Views\LifeBuoyApplication\LifebuoyApplication.cshtml");
            StringBuilder sb = new StringBuilder();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("<input "))
                {
                    string titleStr = "title=\"@Model.";
                    if (!line.Contains(titleStr))
                    {
                        sb.AppendLine(line);
                        continue;
                    }

                    int indexOfFieldStart = line.IndexOf(titleStr) + titleStr.Length;
                    int endIndex = line.IndexOf('.', indexOfFieldStart + 1);
                    string fieldName = line.Substring(indexOfFieldStart, endIndex - indexOfFieldStart);
                    sb.AppendLine(line);
                    string nextLine = file.ReadLine();
                    if (nextLine.Trim() == "<div class=\"input-animation\"></div>")
                    {
                        sb.AppendLine(nextLine);
                    }
                    sb.AppendLine("@if (Sitecore.Context.PageMode.IsExperienceEditor)");
                    sb.AppendLine("{");
                    sb.AppendLine("<div>");
                    sb.AppendLine("Error message: @Html.Sitecore().Field(Model." + fieldName + ".FieldName, Model." + fieldName + ".Item)");
                    sb.AppendLine("</div>");
                    sb.AppendLine("}");
                    /*
                     *  @if (Sitecore.Context.PageMode.IsExperienceEditor)
                                {
                                    <div>
                                        Error message: @Html.Sitecore().Field(Model.OwnershipLabelName.FieldName, Model.OwnershipLabelName.Item)
                                    </div>
                                }
                     */

                }
                else
                {
                    sb.AppendLine(line);
                }
                counter++;
            }

            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  
            string path = @"c:\temp\MyView.cshtml";

            // Create a file to write to.
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
            {
                sw.WriteLine(sb.ToString());
            }
            #endregion

            #region AUTO-GENERATING SITECORE MODEL BASED ON TEMPLATE
            ////DESCRIPTION small code example for auto-generating model properties for a sitecore template, just copy-paste it to a place where you have access to sitecore.context.database and you should be good to go
            ////it could be easily extended to supporting other types of fields as well
            //Sitecore.Data.Templates.Template template = Sitecore.Data.Managers.TemplateManager.GetTemplate(new Sitecore.Data.ID("{DEE049A2-6B13-491E-AC3D-0C29251C8FB8}"), Sitecore.Context.Database); //NEED SITECORE TEMPLATE ID HERE
            //Sitecore.Data.Templates.TemplateField[] allFields = template.GetFields(true).Where(x => !x.Name.Contains("__")).ToArray();
            //System.Text.StringBuilder propertyDefinitions = new System.Text.StringBuilder();
            //System.Text.StringBuilder propertyInitialization = new System.Text.StringBuilder();
            //System.Text.StringBuilder propertyFilling = new System.Text.StringBuilder();

            //foreach (var field in allFields)
            //{
            //    switch (field.Type)
            //    {
            //        case "Multi-Line Text":
            //        case "Single-Line Text":
            //        case "Rich Text":
            //            propertyDefinitions.AppendLine("public ItemAndFieldName " + field.Name + " { get; set; }"); //example: public ItemAndFieldName test { get; set; }
            //            propertyInitialization.AppendLine(field.Name + " = new " + "ItemAndFieldName();");
            //            break;
            //        case "Image":
            //            propertyDefinitions.AppendLine("public ItemAndImageFieldName " + field.Name + " { get; set; }");
            //            propertyInitialization.AppendLine(field.Name + " = new " + "ItemAndImageFieldName();");
            //            break;
            //        case "General Link":
            //            propertyDefinitions.AppendLine("public ItemAndLinkFieldName " + field.Name + " { get; set; }");
            //            propertyInitialization.AppendLine(field.Name + " = new " + "ItemAndLinkFieldName();");
            //            break;
            //        default:
            //            throw new NotImplementedException(field.Type + "Is not currently handled!");
            //            break;
            //    }
            //    propertyFilling.AppendLine(field.Name + ".Item = contextItem;"); //in case the context Item name is contextItem
            //    propertyFilling.AppendLine(field.Name + ".FieldName = LifeBuoyAppItem." + field.Name + "ConstFieldName;"); //CHANGE NAME OF ITEM HERE
            //}

            ////printing to file
            //string pathPropertyDefinitions = @"c:\temp\propertyDefinitions.txt";
            //using (System.IO.StreamWriter sw = System.IO.File.CreateText(pathPropertyDefinitions))
            //{
            //    sw.WriteLine(propertyDefinitions.ToString());
            //}

            //string pathPropertyInitialization = @"c:\temp\propertyInitialization.txt";
            //using (System.IO.StreamWriter sw = System.IO.File.CreateText(pathPropertyInitialization))
            //{
            //    sw.WriteLine(propertyInitialization.ToString());
            //}

            //string pathPropertyFilling = @"c:\temp\propertyFilling.txt";
            //using (System.IO.StreamWriter sw = System.IO.File.CreateText(pathPropertyFilling))
            //{
            //    sw.WriteLine(propertyFilling.ToString());
            //}
            #endregion


            #region SMALLER VERSION OF THE ABOVE CODE
            //if you already have the properties defined you can use the following code example to get the properties with reflection and use that to generate code instead
            /*
            System.Reflection.PropertyInfo[] modelProperties = model.GetType().GetProperties();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var prop in modelProperties)
            {
                //example: startpageHeading = new ItemAndFieldName();
                sb.AppendLine(prop.Name + " = new " + prop.PropertyType.Name + "();");
               
            }
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine("------------------------------------------------------------");
            foreach (var prop in modelProperties)
            {
                   //    example
                   // startpageHeading.Item = contextItem;
                   // startpageHeading.FieldName = LifeBuoyAppItem.startpageHeadingConstFieldName;
                 
            sb.AppendLine(prop.Name + ".Item = contextItem;");
            sb.AppendLine(prop.Name + ".FieldName = LifeBuoyAppItem." + prop.Name + "ConstFieldName;");
             } 
               string path = @"c:\temp\MyTest.txt";

            // Create a file to write to.
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
            {
                sw.WriteLine(sb.ToString());
            }
            */
            #endregion
        }
    }
}
