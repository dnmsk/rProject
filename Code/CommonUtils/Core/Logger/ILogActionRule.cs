using System;
using System.Collections.Generic;

namespace CommonUtils.Core.Logger {
    public interface ILogActionRule {
        /// <summary>
        /// �� ����� ���� �������� �������
        /// </summary>
        List<Enum> ActionsToProcess { get; }

        /// <summary>
        /// ��������� ��������
        /// </summary>
        /// <param name="feature">����</param>
        /// <param name="guestID">��� �������� ��������</param>
        /// <param name="objectId">�������� �����������</param>
        /// <param name="object2Id">�������� ����������� #2, ���� ������ �� �������</param>
        /// <param name="additionalParams"></param>
        void ProcessAction(Enum feature, long guestID, long? objectId = null, long? object2Id = null, Dictionary<string, string> additionalParams = null);
    }
}