<?php

$telemetry = array(
	'key' => '',
	'self' => '',
	'url' => '',
);

function initializeTelemetry() {
	global $telemetry;

	$telemetry['key'] = \OC::$server->getConfig()->getSystemValue('telemetry.instrumentationkey', '');

	if (empty($telemetry['key'])) {
		return;
	}

	require_once 'vendor/autoload.php';
	$telemetry['self'] = $_SERVER['REQUEST_METHOD'] . ' ' . $_SERVER['PHP_SELF'];
	$telemetry['url'] = "http" . (($_SERVER['SERVER_PORT'] == 443) ? "s://" : "://") . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];
}

function executeTelemetry($exception) {
	global $telemetry;

	if (empty($telemetry['key'])) {
		return;
	}

	$time = round((microtime(true) - $_SERVER["REQUEST_TIME_FLOAT"])*1000);
	$client = new \ApplicationInsights\Telemetry_Client();
	$client->getContext()->setInstrumentationKey($telemetry['key']);

	try {
		if ($exception == null) {
			$client->trackRequest($telemetry['self'], $telemetry['url'], time(), $time, 200, true);
		} else {
			$client->trackException($exception);
			$client->trackRequest($telemetry['self'], $telemetry['url'], time(), $time, 500, false);
		}

		$client->flush();

	} catch (Exception $e) {
		error_log($e->getMessage());
	}
}