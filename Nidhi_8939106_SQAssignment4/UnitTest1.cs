using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using SeleniumExtras.WaitHelpers;


namespace Nidhi_8939106_SQAssignment4
{
    public class InsuranceQuoteTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://localhost/prog8171a04/getQuote.html");
        }
        [Test]
        public void TC1_ValidDataAge24Exp3Accidents0()
        {
            FillForm("Nidhi", "Khanpara", "24", "3", "0");
            AssertValidSubmission("$5500");
        }
        [Test]
        public void TC2_ValidDataAge25Exp3Accidents4()
        {
            FillForm("Nidhi", "Khanpara", "25", "3", "4");
            AssertInvalidSubmission("accidents", "No Insurance for you!!  Too many accidents - go take a course!");
           
        }
        [Test]
        public void TC3_ValidDataAge35Exp9Accidents2() 
        {
            FillForm("Nidhi", "Khanpara", "35", "9", "2");

            AssertValidSubmission("$3905");
        }

        [Test]
        public void TC04_InvalidPhoneNumber()
        {
            FillForm("Nidhi", "Khanpara", "27", "3", "0", phoneNumber: "1234567890");
            AssertFieldErrorMessage("phone", "Phone Number must follow the patterns 111-111-1111 or (111)111-1111");
        }

        [Test]
        public void TC05_InvalidEmail()
        {
            FillForm("Nidhi", "Khanpara", "28", "3", "0", email: "invalid@");
            AssertFieldErrorMessage("email", "Must be a valid email address");
        }

        [Test]
        public void TC06_InvalidPostalCode()
        {
            FillForm("Nidhi", "Khanpara", "35", "15", "1", postalCode: "123 456");
            AssertFieldErrorMessage("postalCode", "Postal Code must follow the pattern");
        }
        [Test]
        public void TC07_AgeOmitted()
        {
            FillForm("Nidhi", "Khanpara", "", "5", "0");
            AssertFieldErrorMessage("age", "Age (>=16) is required");
        }
        [Test]
        public void TC08_AccidentsOmitted()
        {
            FillForm("Nidhi", "Khanpara", "37", "8", "");
            AssertFieldErrorMessage("accidents", "Number of accidents is required");
        }
        [Test]
        public void TC09_ExperienceOmitted()
        {
            FillForm("Nidhi", "Khanpara", "45", "", "0");
            AssertFieldErrorMessage("experience", "Years of experience is required");
        }
        [Test]
        public void TC10_NoDrivingExperienceHighAge()
        {
            FillForm("Nidhi", "Khanpara", "50", "0", "0");
            AssertValidSubmission("$7000");
        }

        [Test]
        public void TC11_YoungDriverHighExperience()
        {
            FillForm("FirstName", "Khanpara", "16", "0", "0");
            AssertFieldErrorMessage("experience", "Invalid experience for given age"); 
        }
        [Test]
        public void TC12_ValidDataNoAccidentsOlderDriver()
        {
            FillForm("Nidhi", "Khanpara", "65", "40", "0");
            AssertValidSubmission("$2840");
        }
        [Test]
        public void TC13_YoungDriverManyAccidents()
        {
            FillForm("Nidhi", "Khanpara", "20", "2", "5");
            AssertFieldErrorMessage("accidents", "No Insurance for you!!");

        }
        [Test]
        public void TC14_InvalidAgeTooLow()
        {
            FillForm("Nidhi", "Khanpara", "15", "0", "0");
            AssertFieldErrorMessage("age", "Age must be 16 or older");
        }
        [Test]
        public void TC15_MaxExperienceNewDriver()
        {
            FillForm("Nidhi", "Khanpara", "35", "19", "0");
            AssertFieldErrorMessage("experience", "Invalid experience for given age"); 
        }


            private void FillForm(
            string firstName,
            string lastName,
            string age,
            string experience,
            string accidents,
            string address = "102 University Ave",
            string city = "Waterloo",
            string province = "ON",
            string postalCode = "N2K 4A2",
            string phoneNumber = "102-000-0000",
            string email = "nidhikhanpara@mail.com")
        {
            driver.FindElement(By.Id("firstName")).SendKeys(firstName);
            driver.FindElement(By.Id("lastName")).SendKeys(lastName);
            driver.FindElement(By.Id("address")).SendKeys(address);
            driver.FindElement(By.Id("city")).SendKeys(city);
            new SelectElement(driver.FindElement(By.Id("province"))).SelectByText(province);
            driver.FindElement(By.Id("postalCode")).SendKeys(postalCode);
            driver.FindElement(By.Id("phone")).SendKeys(phoneNumber);
            driver.FindElement(By.Id("email")).SendKeys(email);
            driver.FindElement(By.Id("age")).SendKeys(age);
            driver.FindElement(By.Id("experience")).SendKeys(experience);
            driver.FindElement(By.Id("accidents")).SendKeys(accidents);
            driver.FindElement(By.Id("btnSubmit")).Click();
        }

        private void AssertValidSubmission(string expectedQuote)
        {
            var finalQuote = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("finalQuote")))[0].GetAttribute("value");
            Assert.AreEqual(expectedQuote, finalQuote, $"Expected quote was {expectedQuote}, but found {finalQuote}.");
        }
        private void AssertInvalidSubmission(string fieldId, string expectedErrorMessage)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(driver => driver.FindElements(By.Id($"{fieldId}-error")).Count > 0);

                string actualErrorMessage = driver.FindElement(By.Id($"{fieldId}-error")).Text;
                Assert.IsTrue(actualErrorMessage.Contains(expectedErrorMessage), $"Expected error message for '{fieldId}' to be '{expectedErrorMessage}', but was '{actualErrorMessage}'.");
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail($"Timed out waiting for the error message for '{fieldId}' to appear.");
            }
        }


        private void AssertFieldErrorMessage(string fieldId, string expectedErrorMessage)
        {
            string errorElementId = $"{fieldId}-error";
            var errorMessageElement = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(errorElementId)))[0];
            string actualErrorMessage = errorMessageElement.Text;
            Assert.IsTrue(actualErrorMessage.Contains(expectedErrorMessage), $"Expected error message for '{fieldId}' to be '{expectedErrorMessage}', but was '{actualErrorMessage}'.");
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}



