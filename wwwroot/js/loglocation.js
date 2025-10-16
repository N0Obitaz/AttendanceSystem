 document.getElementById("btnLocation").addEventListener("click", async () => {
            if (!navigator.geolocation) {
                alert("Geolocation not supported.");
                return;
            }

            navigator.geolocation.getCurrentPosition(async (pos) => {
                console.log("Position:", pos.coords);

                const lat = pos.coords.latitude.toFixed(6);
                const lon = pos.coords.longitude.toFixed(6);
                const acc = pos.coords.accuracy.toFixed(2);

                document.getElementById("lat").innerText = lat;
                document.getElementById("lon").innerText = lon;
                document.getElementById("acc").innerText = acc;

                const data = {
                    latitude: pos.coords.latitude,
                    longitude: pos.coords.longitude,
                    accuracy: pos.coords.accuracy
                };

                const response = await fetch("?handler=LogLocation", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(data)
                });

                let result;
                try {
                    result = await response.json();

                    if (result.success) {
                        var cancelBtn = document.getElementById("cancelBtn");

                        cancelBtn.style.display = "inline-block";
                        document.getElementById("locationResult").innerText = result.message;
                        document.getElementById("locationSuccess").innerText = "Please wait for your attendance to be marked"
                        setTimeout(() => window.location.reload(), 5000);

                    }
                } catch (e) {
                    console.error("Invalid JSON from server:", e);
                    const text = await response.text();
                    console.log("Raw response:", text);
                    document.getElementById("locationResult").innerText = "Server returned invalid JSON.";
                    return;
                }
      
               
               

            }, (err) => {
                console.error("Geolocation error:", err);
                // Proper error handling for PositionError
                let errorMessage = "Error getting Location: ";
                switch (err.code) {
                    case err.PERMISSION_DENIED:
                        errorMessage += "Location access denied by user. Please allow location permissions and try again.";
                        break;
                    case err.POSITION_UNAVAILABLE:
                        errorMessage += "Location information unavailable. Check if GPS is enabled.";
                        break;
                    case err.TIMEOUT:
                        errorMessage += "Location request timed out. Please try again.";
                        break;
                    default:
                        errorMessage += "An unknown error occurred.";
                        break;
                }
                alert(errorMessage);
                document.getElementById("locationResult").innerText = errorMessage;
            }, {
                enableHighAccuracy: true,
                timeout: 5000, // 10 seconds
                maximumAge: 0 // Don't use cached position
            });
 });


