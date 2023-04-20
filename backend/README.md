	dotnet user-secrets init --project Flashcards
	
	dotnet user-secrets set "JsonBin:ApiKey" 'YOUR SECRET HERE' --project "Flashcards"
	
	aws ecr get-login-password --region eu-west-1 --profile personal
	docker login --username AWS --password PASSWORD_HERE 915136666174.dkr.ecr.eu-west-1.amazonaws.com
	docker build -f .\Flashcards\Dockerfile -t flashcards .
	docker tag flashcards:latest 915136666174.dkr.ecr.eu-west-1.amazonaws.com/flashcards:latest
	docker push 915136666174.dkr.ecr.eu-west-1.amazonaws.com/flashcards:latest