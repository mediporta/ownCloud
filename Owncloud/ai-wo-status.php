<?php

require_once 'vendor/autoload.php';

$url = "http" . (($_SERVER['SERVER_PORT'] == 443) ? "s://" : "://") . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];
$telemetryUrlSelf = $_SERVER['PHP_SELF'];

executeTelemetry(null);

function executeTelemetry($telemetryException){
	$timePassed = round((microtime(true) - $_SERVER["REQUEST_TIME_FLOAT"])*1000);
	$telemetryClient = new \ApplicationInsights\Telemetry_Client();

	//Hardcoded instrumentation key for testing purposes only
	$telemetryClient->getContext()->setInstrumentationKey('b06239d5-cfea-404a-8247-ed0d3b04fe1d');

	if($telemetryException != null) {
		$telemetryClient->trackException($ex);
		$telemetryClient->trackRequest(isset($telemetryUrlSelf) ? $telemetryUrlSelf : '', isset($url) ? $url : '', time(), $timePassed, 500, false);
	} else {
		$telemetryClient->trackRequest(isset($telemetryUrlSelf) ? $telemetryUrlSelf : '', isset($url) ? $url : '', time(), $timePassed, 200, true);
	}
	$telemetryClient->flush();
}