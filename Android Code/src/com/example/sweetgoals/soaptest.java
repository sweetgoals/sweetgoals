package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;
import android.widget.Toast;

public class soaptest extends Activity{
    
    TextView test1, test2, test3, test4,test5,test6; 
    
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);   	
        setContentView(R.layout.soaptest);
//        test1 = (TextView)findViewById(R.id.TextView01);
//        test2 = (TextView)findViewById(R.id.TextView02);
//
//        SoapObject request = new SoapObject("http://tempuri.org/", "checkUser");
//        request.addProperty("user","dash");
//        request.addProperty("pass", "blah");
//        
//        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
//        envelope.setOutputSoapObject(request);
//        envelope.dotNet = true;
//		Toast.makeText(getApplicationContext(), "1", Toast.LENGTH_LONG).show();
//        try {
//        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.letstrend.com/spursService.asmx?WSDL");
//        	androidHttpTransport.call("http://tempuri.org/checkUser", envelope);
//    		SoapObject result = (SoapObject)envelope.bodyIn;
//        	test1.setText(result.getProperty(0).toString());
//        	test2.setText(result.getProperty(1).toString());
//        } catch (Exception e) {
//        	e.printStackTrace();
//        	Toast.makeText(getApplicationContext(), "caught", Toast.LENGTH_LONG).show();
//        }
    }  

}
