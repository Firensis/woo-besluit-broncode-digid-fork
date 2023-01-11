// Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
// gericht is op transparantie en niet op hergebruik. Hergebruik van 
// de broncode is toegestaan onder de EUPL licentie, met uitzondering 
// van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
//
// This code has been disclosed in response to a request under the Dutch
// Open Government Act ("Wet open Overheid"). This implies that publication 
// is primarily driven by the need for transparence, not re-use.
// Re-use is permitted under the EUPL-license, with the exception 
// of source files that contain a different license.
//
// The archive that this file originates from can be found at:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// using the reference "Logius, publicly disclosed source code DigiD-App" 
//
// Other questions regarding this Open Goverment Act decision may be
// directed via email to open@logius.nl
//
﻿using System.Threading.Tasks;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Apdu;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <summary>
    /// The client sends an APDU to the PCA,
    /// to obtain the Card Agreed public key.
    /// </summary>
    internal class StepGetCardAgreementPublicKey : IStep
    {
        private readonly Gap _gap;

        public StepGetCardAgreementPublicKey(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            var xy = SecurityHelper.GetXandYPoints((ECPublicKeyParameters)_gap.Pace.AgreementKeyPair.Public);

            var command = CommandApduBuilder.GetPairAgreementCommand(xy.x, xy.y, _gap.SMContext);
            var response = await CommandApduBuilder.SendAPDU("PACE GetCardAgreementPublicKey", command, _gap.SMContext);

            if (response.SW == 0x9000)
            {
                var agreementResp = new XYResponseAPDU(response);
                _gap.Pace.CardAgreedPublicKey = SecurityHelper.DecodeKey(agreementResp, _gap.Card.EF_CardAccess.PaceInfo.Algorithm);

                return true;
            }

            return false;
        }
    }
}