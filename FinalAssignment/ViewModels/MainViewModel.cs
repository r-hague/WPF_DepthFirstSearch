using FinalAssignment.DataAccess.Providers;
using FinalAssignment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace FinalAssignment.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string inputText;
        private string outputText;

        private bool isNeedToProccessInput = false;

        List<string> OutString = new List<string>();

        #region Properties

        public string InputText
        {
            get
            {
                return this.inputText;
            }
            set
            {
                if (this.inputText != value)
                {
                    this.inputText = value;
                }
            }
        }

        public string OutputText
        {
            get
            {
                return this.outputText;
            }
            set
            {
                if (this.outputText != value)
                {
                    this.outputText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        public void LoadInput(string fileLocation)
        {
            var listOfValues = FileProvider.ReadFile(fileLocation);

            if (listOfValues != null)
            {
                this.InputText = listOfValues;
            }
        }

        public void ProcessInput()
        {
            if (!string.IsNullOrEmpty(this.InputText))
            {
                this.isNeedToProccessInput = false;

                List<string> ResultList = new List<string>();

                if (Regex.IsMatch(InputText, @"^(([1-9]|1[0-9]|2[0-1])[\n](([1-9]|1[0-9]|2[0-1]) ([1-9]|1[0-9]|2[0-1])[\n])+[0]{1} [0]{1}[\n])+[\r][\n]$", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(InputText, @"^(([1-9]|1[0-9]|2[0-1])[\n](([1-9]|1[0-9]|2[0-1]) ([1-9]|1[0-9]|2[0-1])[\n])+[0]{1} [0]{1})+[\r][\n]$", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(InputText, @"^(([1-9]|1[0-9]|2[0-1])[\r][\n](([1-9]|1[0-9]|2[0-1]) ([1-9]|1[0-9]|2[0-1])[\r][\n])+[0]{1} [0]{1})+[\r][\n]$", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(InputText, @"^(([1-9]|1[0-9]|2[0-1])[\r][\n](([1-9]|1[0-9]|2[0-1]) ([1-9]|1[0-9]|2[0-1])[\r][\n])+[0]{1} [0]{1}[\r][\n])+[\r][\n]$", RegexOptions.IgnoreCase))
                {
                    var inputString = InputText.Replace("\n\r\n", "");
                    var inputGroupedArray = inputString.Split(new string[] { "\n0 0" }, StringSplitOptions.RemoveEmptyEntries);


                    List<Case> caseList = new List<Case>();
                    int caseNumber = 0;

                    foreach (var group in inputGroupedArray)
                    {
                        var groupArray = group.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        Case tempCase = null;

                        foreach (var groupValue in groupArray)
                        {
                            //remove unnecessary \r symbols
                            var tempStr = groupValue.Replace("\r", "");
                            string[] numbers = tempStr.Split(' ');

                            if (numbers.Length == 1)
                            {
                                if (!string.IsNullOrEmpty(numbers[0]))
                                {
                                    //final street
                                    caseNumber += 1;
                                    tempCase = new Case()
                                    {
                                        CaseNumber = caseNumber,
                                        FireCorner = Convert.ToInt32(numbers[0]),
                                        StreetList = new Dictionary<int, List<int>>()
                                    };
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(tempStr) && tempCase != null)
                                {
                                    int firstNumber = Convert.ToInt32(numbers[0]);
                                    int secondNumber = Convert.ToInt32(numbers[1]);

                                    if (firstNumber != 0 && secondNumber != 0)
                                    {
                                        this.AddToDict(ref tempCase, secondNumber, firstNumber);
                                        this.AddToDict(ref tempCase, firstNumber, secondNumber);
                                    }
                                }
                            }
                        }

                        caseList.Add(tempCase);
                    }

                    this.CalculateOutput(caseList);
                }
                else
                {
                    //input string has wrong formatting
                    this.OutputText = string.Empty;
                    this.InputText = string.Empty;
                    MessageBox.Show($"Input values has wrong formatting", "Input error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.isNeedToProccessInput = true;
            }
        }

        private void AddToDict(ref Case tempCase, int key, int value)
        {
            var isContainKey = tempCase.StreetList.ContainsKey(key);
            List<int> outValueList;

            if (isContainKey)
            {
                if (tempCase.StreetList.TryGetValue(key, out outValueList))
                {
                    outValueList.Add(value);
                    tempCase.StreetList[key] = outValueList;
                }
            }
            else
            {
                tempCase.StreetList.Add(key, new List<int>() { value });
            }
        }

        private void CalculateOutput(List<Case> caseList)
        {
            if (isNeedToProccessInput)
            {
                this.ProcessInput();
            }

            var results = new Dictionary<int, List<List<int>>>();
            foreach (var tempCase in caseList)
            {
                var groupResultValues = new List<List<int>>();

                List<int> outValueList;
                tempCase.StreetList.TryGetValue(1, out outValueList);

                outValueList.Sort();
                foreach (var endPoint in outValueList)
                {
                    groupResultValues.Add(new List<int>() { 1, endPoint });
                }

                var isNeedToMove = true;
                while (isNeedToMove)
                {
                    var copiedResultValues = groupResultValues.ToList();

                    foreach (var consequence in copiedResultValues)
                    {
                        if (consequence.Last() != tempCase.FireCorner)
                        {
                            List<int> nextPoints;
                            if (tempCase.StreetList.TryGetValue(consequence.Last(), out nextPoints))
                            {
                                var uniqueNumbers = nextPoints.Where(number => !consequence.Contains(number)).ToList();
                                var localResults = new List<List<int>>();

                                if (uniqueNumbers.Count == 0)
                                {
                                    groupResultValues.Remove(consequence);
                                }
                                else
                                {
                                    var consequenceIndex = groupResultValues.IndexOf(consequence);

                                    uniqueNumbers.Sort();

                                    foreach (var nextCornerValue in uniqueNumbers)
                                    {
                                        var newConsequence = new List<int>(consequence);
                                        newConsequence.Add(nextCornerValue);


                                        groupResultValues.Insert(consequenceIndex, newConsequence);
                                        consequenceIndex++;
                                    }

                                    groupResultValues.Remove(consequence);
                                }

                            }
                            else groupResultValues.Remove(consequence);

                        }
                    }

                    isNeedToMove = groupResultValues.FirstOrDefault(item => item.Last() != tempCase.FireCorner) != null;


                }

                if (groupResultValues.Count == 0)
                {
                    MessageBox.Show($"Group numeber {tempCase.CaseNumber} with corner {tempCase.FireCorner} does not have any results",
                                      "Information",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    continue;
                }

                results.Add(tempCase.CaseNumber, groupResultValues);
            }

            var resultString = string.Empty;
            foreach (var result in results)
            {
                resultString += $"CASE {result.Key}: {Environment.NewLine}";
                foreach (var item in result.Value)
                {
                    foreach (var value in item)
                    {
                        resultString += $"{value} ";
                    }
                    resultString += Environment.NewLine;
                }

                resultString += $"There are {result.Value.Count} routes from the fire station to street corner {result.Value.First().Last()}.{Environment.NewLine}{Environment.NewLine}";
            }

            this.OutputText = resultString;
        }

        #region propertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
