<?php
/**
 * @author Andreas Fischer <bantu@owncloud.com>
 * @author Christopher Schäpers <kondou@ts.unde.re>
 * @author Frank Karlitschek <frank@karlitschek.de>
 * @author Joas Schilling <coding@schilljs.com>
 * @author Jörn Friedrich Dreyer <jfd@butonic.de>
 * @author Kristof Provost <github@sigsegv.be>
 * @author Lukas Reschke <lukas@statuscode.ch>
 * @author martin.mattel@diemattels.at <martin.mattel@diemattels.at>
 * @author Masaki Kawabata Neto <masaki.kawabata@gmail.com>
 * @author Morris Jobke <hey@morrisjobke.de>
 * @author Philipp Schaffrath <github@philippschaffrath.de>
 * @author Thomas Müller <thomas.mueller@tmit.eu>
 *
 * @copyright Copyright (c) 2017, ownCloud GmbH
 * @license AGPL-3.0
 *
 * This code is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License, version 3,
 * as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License, version 3,
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 *
 */

$url = "http" . (($_SERVER['SERVER_PORT'] == 443) ? "s://" : "://") . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];
$telemetryException = null;
$telemetryTimeStart = null;
$telemetryTimeEnd = null;
$telemetryUrlSelf = $_SERVER['PHP_SELF'];

try {

	$telemetryTimeStart = round(microtime(true) * 1000);

	require_once __DIR__ . '/lib/base.php';

	# show the version details based on config.php parameter, 
	# but do not expose the servername in the public via url
	$values = \OCP\Util::getStatusInfo(
		null,
		\OC::$server->getConfig()->getSystemValue('show_server_hostname', false) !== true);

	if (OC::$CLI) {
		print_r($values);
	} else {
		header('Access-Control-Allow-Origin: *');
		header('Content-Type: application/json');
		echo json_encode($values);
	}

} catch (Exception $ex) {
	OC_Response::setStatus(OC_Response::STATUS_INTERNAL_SERVER_ERROR);
	\OCP\Util::writeLog('remote', $ex->getMessage(), \OCP\Util::FATAL);
	trackExceptionWithTelemetry($ex);
}

function executeTelemetry(){
	$telemetryTimeEnd = round(microtime(true) * 1000);
	$timePassed = $telemetryTimeEnd - $telemetryTimeStart;
	$telemetryClient = new \ApplicationInsights\Telemetry_Client();
	$telemetryClient->getContext()->setInstrumentationKey(\OC::$server->getConfig()->getSystemValue('azure.instrumentationkey'));

	if($telemetryException != null) {
		$telemetryClient->trackException($ex);
		$telemetryClient->trackRequest($telemetryUrlSelf, $url, time(), $timePassed, 500, false);
	} else {
		$telemetryClient->trackRequest($telemetryUrlSelf, $url, time(), $timePassed, 200, true);
	}
	$telemetryClient->flush();
}

function trackExceptionWithTelemetry(Exception $ex) {
	$telemetryException = $ex;
	executeTelemetry();
}