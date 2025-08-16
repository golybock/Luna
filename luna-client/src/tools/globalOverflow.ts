export class GlobalOverflow{
	public static hide(){
		document.body.classList.add("modalOpen");
	}

	public static show(){
		document.body.classList.remove("modalOpen");
	}

	public static isHidden(): boolean {
		return document.body.classList.contains("modalOpen");
	}
}