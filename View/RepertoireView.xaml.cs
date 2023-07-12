using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicBand_Manager.View
{
    public partial class RepertoireView : Page
    {
        private static RepertoireView _instance;

        public static RepertoireView Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RepertoireView();

                return _instance;
            }
        }

        private RepertoireView()
        {
            InitializeComponent();
        }
    }
}
