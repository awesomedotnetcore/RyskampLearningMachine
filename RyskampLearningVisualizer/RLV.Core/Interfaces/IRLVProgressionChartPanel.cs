﻿using RLM.Models;
using RLV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLV.Core.Interfaces
{
    public interface IRLVProgressionChartPanel : IRLVPanel
    {
        IRLVScaleSelectionPanel ScalePanel { get; }

        event SelectedCaseChangedDelegate SelectedCaseChangedEvent;
        event SelectedCaseScaleChangedDelegate SelectedCaseScaleChangedEvent;
        event SelectChartDataPointDelegate SelectChartDataPointEvent;

        void IRLVCore_NextPrevCaseChangedResultsHandler(long caseId);
        void IRLVCore_SelectedUniqueInputSetChangedResultsHandler(IEnumerable<RlmLearnedCase> data, IEnumerable<IRLVItemDisplay> itemDisplay, bool showComparison = false);
        void IRLVCore_ScaleChangedResultsHandler(IEnumerable<RlmLearnedCase> data);
        void IRLVCore_RealTimeUpdateHandler(IEnumerable<RlmLearnedCase> data);
        void IRLVCore_SelectedCaseScaleChangedResultsHandler(IEnumerable<RlmLearnedCase> data, long selectedCaseId);
        void IRLVProgressionChartChangeScale(long caseId, double scale);
        void IRLVProgressionChartSelectCase(long caseId);
    }
}
