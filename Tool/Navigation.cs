using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicBand_Manager.Tool
{
    public class Navigation
    {
        private Frame _mainFrame;
        private static Navigation _instance;
        private static readonly object _lockObject = new object();
        public static Navigation Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Navigation();
                        }
                    }
                }
                return _instance;
            }
        }

        private Navigation()
        {
            // Make the constructor private to prevent direct instantiation
        }

        public void Initialize(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        public void NavigateTo(string nameOfPage)
        {
            // Find the corresponding type using reflection
            Type pageType = Type.GetType($"MusicBand_Manager.View.{nameOfPage}, MusicBand_Manager");

            if (pageType != null)
            {
                // Create an instance of the page using the Singleton pattern
                var pageInstance = pageType.GetProperty("Instance").GetValue(null, null);

                // Set the content of the frame to the page instance
                _mainFrame.Content = pageInstance;
            }
        }
        public Page CurrentPage
        {
            get { return _mainFrame.Content as Page; }
        }

        public System.Type CurrentPageType
        {
            get { return _mainFrame.Content.GetType(); }
        }
    }

}
