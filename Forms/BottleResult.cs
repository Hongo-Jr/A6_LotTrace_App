using LotTraceApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotTraceApp.Forms
{
    public partial class BottleResult : Form
    {
        private readonly BottleResultService _service;

        public BottleResult(BottleResultService service,string orderNumber, string lotNo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));

            InitializeComponent();
        }
    }
}
