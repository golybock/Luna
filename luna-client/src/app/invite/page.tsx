"use server";

import { InvitePage } from "@/components/pages/invite/InvitePage";

export default async function pageContainer({ searchParams }: { searchParams: Promise<{ inviteId: string }> }) {
	const { inviteId } = await searchParams;
	return <InvitePage inviteId={inviteId}/>
}